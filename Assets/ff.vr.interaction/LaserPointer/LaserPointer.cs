using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
//using ff.vr.annotate;
using ff.nodegraph;
using ff.nodegraph.interaction;

namespace ff.vr.interaction
{
    public interface ILaserPointerTarget
    {
        void PointerEnter(LaserPointer pointer);
        void PointerExit(LaserPointer pointer);
        void PointerUpdate(LaserPointer pointer);
    }

    public interface IClickableLaserPointerTarget : ILaserPointerTarget
    {
        void PointerTriggered(LaserPointer pointer);
        void PointerUntriggered(LaserPointer pointer);
    }

    public interface IHoverColor
    {
        Color GetHoverColor();
    }

    public interface ILaserPointerAppearance
    {
        Color LaserColor { get; }
        Color HighlightColor { get; }
        GameObject HitIndicator { get; }
    }

    public interface ILaserPointerHighlightable
    {
        void SetHighlightColor(Color highlightColor);
        void ShowHighlight();
        void HideHighlight();
    }


    public class LaserPointer : MonoBehaviour
    {
        public delegate void PointingAtChanged(object sender, PointingAtChangedEventArgs e);

        public event PointingAtChanged PointingAtChangedEvent;
        public event PointingAtChanged NewTargetEnteredEvent;

        public Material NonHitMaterial;
        public Color HoverColor = Color.blue;
        [HideInInspector]
        public LaserPointerStyle Style;

        [HideInInspector]
        public InteractiveController Controller;

        [HideInInspector]
        public Vector3 LastHitPoint;

        private float LastHitDistance = 100;

        [HideInInspector]
        public Ray Ray;

        [HideInInspector]
        public bool IsLockedAtTarget;

        private NodeSelectionManager _hitTester;

        void Start()
        {
            if (_laserHitSphere == null)
                _laserHitSphere = this.transform.GetChild(0).gameObject;

            if (_laserHitSphere)
            {
                _laserHitSphereMaterial = _laserHitSphere.GetComponent<Renderer>().material;
            }
            _hitTester = FindObjectOfType<NodeSelectionManager>();
            if (_hitTester == null)
            {
                Debug.LogError("NodeHitTester not found in scene");
            }

            _lineRenderer = GetComponent<LineRenderer>();
            _lineMaterial = GetComponent<Renderer>().material;
        }

        private void Update()
        {
            if (!_laserIsEnabled)
                return;

            ILaserPointerTarget newTarget = FindHitTarget();

            SetNewHitTarget(newTarget);
            SetLaserLength(LastHitDistance);

            _laserHitSphere.transform.position = LastHitPoint;
        }



        private ILaserPointerTarget FindHitTarget()
        {
            ILaserPointerTarget newTarget = null;

            Ray = new Ray(transform.position, transform.forward);
            //LayerMask layerMask = LayerMask.GetMask(new[] { "_AnnotationTarget", "_TeleportationTarget" });

            RaycastHit physicsHit = new RaycastHit();
            var hasPhysicsHit = (Physics.Raycast(Ray, out physicsHit, 1000));

            var nodeHit = (_hitTester != null) ? _hitTester.FindAndRememberHit(Ray) : null;
            var hasNodeHit = (nodeHit != null);

            // Physics ray wins
            if (hasPhysicsHit && (!hasNodeHit || nodeHit.HitDistance > physicsHit.distance - 0.5f))
            {
                LastHitPoint = physicsHit.point;
                LastHitDistance = physicsHit.distance;

                var hitCollider = physicsHit.collider;
                var newTargetInterface = hitCollider.attachedRigidbody
                                     ? hitCollider.attachedRigidbody.GetComponent<ILaserPointerTarget>()
                                     : hitCollider.gameObject.GetComponent<ILaserPointerTarget>();
                newTarget = newTargetInterface as ILaserPointerTarget;
            }
            // NodeHit wins...
            else if (hasNodeHit)
            {
                LastHitPoint = Ray.origin + Ray.direction * nodeHit.HitDistance;
                LastHitDistance = nodeHit.HitDistance;
                newTarget = _hitTester as ILaserPointerTarget;
            }
            // Nothing hit
            else
            {
                LastHitDistance = 100;
                LastHitPoint = Ray.direction * 100;
            }

            return newTarget;
        }

        private void SetNewHitTarget(ILaserPointerTarget newTarget)
        {
            var targetIsSame = (newTarget == PointingAt || IsLockedAtTarget);
            if (targetIsSame)
            {
                // Update position on Target
                if (PointingAt != null)
                {
                    PointingAt.PointerUpdate(this);
                }
                return;
            }

            if (Controller.ActiveState == InteractiveController.States.PointerCapturedOnClickable)
                return;

            // Exit old target
            if (PointingAt != null && !PointingAt.Equals(null)) // Also checking with .Equals because ==Operator is not overridden from MonoBehaviour
            {
                PointingAt.PointerExit(this);
            }

            PointingAt = newTarget;
            UpdateStyleIfTargetChanged();

            if (NewTargetEnteredEvent != null)
            {
                NewTargetEnteredEvent(this, new PointingAtChangedEventArgs(PointingAt, newTarget));
            }

            if (newTarget != null)
            {
                newTarget.PointerEnter(this);
            }
        }


        private void UpdateStyleIfTargetChanged()
        {
            var newColor = _inactiveStyle.LaserColor;
            var spotSize = NON_TARGET_SPOT_SIZE;
            var newLineWidth = INVALID_TARGET_LINE_WIDTH;

            newLineWidth = VALID_TARGET_LINE_WIDTH;
            spotSize = TARGET_SPOT_SIZE;

            var hoverColorProvider = PointingAt as IHoverColor;
            if (hoverColorProvider != null)
            {
                newColor = hoverColorProvider.GetHoverColor();
            }
            else
            {
                newColor = Style != null ? Style.LaserColor : HoverColor;
            }

            _lineMaterial.color = newColor;
            _laserHitSphereMaterial.color = newColor;

            _lineRenderer.startWidth = newLineWidth;
            _lineRenderer.endWidth = newLineWidth;

            var positiveDistance = Mathf.Abs(LastHitDistance);  // Can be negative inside bounding boxes
            float scaleByDistance = Mathf.Sqrt(positiveDistance / 2) * spotSize;
            _laserHitSphere.transform.localScale = new Vector3(scaleByDistance, scaleByDistance, scaleByDistance);
        }



        public void SetLaserLength(float distance)
        {
            _lineRenderer.SetPosition(1, new Vector3(0, 0, distance));
        }

        // Note: We have to return a list because RaycastHit is not nullable
        public RaycastHit[] GetRayHitsForComponent<T>()
        {
            var hits = Physics.RaycastAll(Ray, 100f).OrderBy(hit => hit.distance);
            var hitsWithPlacementPlanes = new List<RaycastHit>();
            foreach (RaycastHit hit in hits)
            {
                var matchingComponent = hit.collider.gameObject.GetComponent<T>();
                if (matchingComponent != null)
                    hitsWithPlacementPlanes.Add(hit);
            }
            return hitsWithPlacementPlanes.ToArray();
        }



        public void SetLaserpointerEnabled(bool enabled)
        {
            if (!enabled && PointingAt != null)
            {
                PointingAt.PointerExit(this);
                PointingAt = null;
            }
            SetLaserVisible(enabled);
            _laserIsEnabled = enabled;
        }


        private void SetLaserVisible(bool visible)
        {
            _lineRenderer.enabled = visible;
            _laserHitSphere.SetActive(visible);
        }

        private ILaserPointerTarget _pointingAt;

        public ILaserPointerTarget PointingAt
        {
            get { return _pointingAt; }
            set
            {
                if (_pointingAt == value)
                    return;

                var old = _pointingAt;
                _pointingAt = value;

                if (PointingAtChangedEvent != null)
                    PointingAtChangedEvent(this, new PointingAtChangedEventArgs(old, _pointingAt));
            }
        }

        const float VALID_TARGET_LINE_WIDTH = 0.01f;
        const float INVALID_TARGET_LINE_WIDTH = 0.004f;

        const float TARGET_SPOT_SIZE = 0.02f;
        const float NON_TARGET_SPOT_SIZE = 0.01f;

        private bool _laserIsEnabled = true;
        public GameObject _laserHitSphere;
        private LineRenderer _lineRenderer;
        private Material _lineMaterial;
        private Material _laserHitSphereMaterial;
        private LaserPointerStyle _inactiveStyle = new InactiveLaserPointerStyle();
    }

    public class PointingAtChangedEventArgs : EventArgs
    {
        public ILaserPointerTarget OldTarget { get; private set; }

        public ILaserPointerTarget NewTarget { get; private set; }

        public PointingAtChangedEventArgs(ILaserPointerTarget oldTarget, ILaserPointerTarget newTarget)
        {
            OldTarget = oldTarget;
            NewTarget = newTarget;
        }
    }
}
