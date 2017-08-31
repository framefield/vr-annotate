using UnityEngine;
using System.Collections.Generic;
using System;
using ff.vr.annotate.viz;
using ff.nodegraph.interaction;

namespace ff.vr.interaction
{
    /// <summary>
    /// This Compononent handles the interaction with Vive Controllers for teleportation, 
    /// annotations, drag-gizmos, buttons, etc.
    /// </summary>

    [RequireComponent(typeof(SteamVR_TrackedController))]
    public class InteractiveController : MonoBehaviour
    {
        public GameObject LaserPrefab;

        // This is override by derived classes
        protected virtual void Start()
        {
            Initialize();
        }

        protected void Initialize()
        {
            if (_controllerInitialized)
                return;

            _laserInstance = Instantiate(LaserPrefab);
            _laserInstance.transform.SetParent(transform, false);
            _laserPointer = _laserInstance.GetComponent<LaserPointer>();
            _laserPointer.Controller = this;

            _teleportation = GetComponent<Teleportation>();
            _audioSourceForTeleportation = GetComponent<AudioSource>();
            _controllerInitialized = true;
        }
        private bool _controllerInitialized = false;

        private AudioSource _audioSourceForTeleportation;

        protected virtual void OnEnable()
        {
            // Setup EventHandlers
            _controller = GetComponent<SteamVR_TrackedController>();
            if (_controller == null)
            {
                Debug.LogError("SteamVR_Teleporter must be on a SteamVR_TrackedController");
                return;
            }
            _controller.TriggerClicked += TriggerClickedHandler;
            _controller.TriggerUnclicked += TriggerUnclickedHandler;
            _controller.PadClicked += PadClickedHandler;
            _controller.PadUnclicked += PadUnclickedHandler;
            _controller.Gripped += ControllerGrippedHandler;

            UpdateListofGizmoColliders();
        }


        protected virtual void OnDisable()
        {
            if (_controller == null)
                return;

            _controller.TriggerClicked -= TriggerClickedHandler;
            _controller.TriggerUnclicked -= TriggerUnclickedHandler;
            _controller.PadClicked -= PadClickedHandler;
            _controller.PadUnclicked -= PadUnclickedHandler;
            _controller.Gripped -= ControllerGrippedHandler;
        }


        public void UpdateListofGizmoColliders()
        {
            foreach (var h in FindObjectsOfType<InteractiveGizmo>())
            {
                var collider = h.GetComponent<BoxCollider>();
                if (collider != null)
                {
                    GizmoColliders.Add(collider);
                }
            }
        }

        private List<BoxCollider> GizmoColliders = new List<BoxCollider>();


        protected virtual void Update()
        {
            // Debug.Log(_state);
            // Find current hover interactive handle
            var newHoverGizmo = GetGizmoUnderController();

            if (_state == States.Default)
            {
                if (newHoverGizmo)
                {
                    newHoverGizmo.OnControllerEnter(this);
                    _state = States.IsCollidingWithGizmo;
                    _currentHoverGizmo = newHoverGizmo;
                    _laserPointer.SetLaserpointerEnabled(false);
                }
            }
            else if (_state == States.IsCollidingWithGizmo)
            {
                if (newHoverGizmo == _currentHoverGizmo)
                {
                    _currentHoverGizmo.OnControllerStay(this);
                }
                else
                {
                    _currentHoverGizmo.OnControllerExit(this);
                    _state = States.Default;
                    _currentHoverGizmo = null;
                    _laserPointer.SetLaserpointerEnabled(true);
                }
            }
            else if (_state == States.DraggingGizmo)
            {
                _currentHoverGizmo.OnDragUpdate(this);
            }
            else if (_state == States.PointerCapturedOnClickable && !(_laserPointer.PointingAt is IClickableLaserPointerTarget))
            {
                _capturedClickTarget = null;
                _state = States.Default;
                _laserPointer.IsLockedAtTarget = false;
            }
            else if (_state == States.PointerCapturedOnTeleporter && !(_laserPointer.PointingAt is AnnotationPositionTeleporter))
            {
                _capturedAnnotationTeleporter.SetPerspectiveHighlight(false);
                _capturedAnnotationTeleporter = null;
                _state = States.Default;
            }
        }

        private void TriggerClickedHandler(object sender, ClickedEventArgs clickedEventArgs)
        {
            if (_laserPointer == null)
            {
                Debug.LogError("LaserPointer component not found for interactiveController");
                return;
            }

            if (_state == States.Default)
            {
                if (_laserPointer.PointingAt is IClickableLaserPointerTarget)
                {
                    _capturedClickTarget = _laserPointer.PointingAt as IClickableLaserPointerTarget;
                    _capturedClickTarget.PointerTriggered(_laserPointer);
                    _state = States.PointerCapturedOnClickable;
                    _laserPointer.IsLockedAtTarget = true;
                }
                else
                {
                    NodeSelector.Instance.SelectParentNode();
                }
            }
            else if (_state == States.IsCollidingWithGizmo)
            {
                if (_currentHoverGizmo.OnDragStarted(this))
                    _state = States.DraggingGizmo;
            }
        }


        private void TriggerUnclickedHandler(object sender, ClickedEventArgs clickedEventArgs)
        {
            if (_state == States.DraggingGizmo)
            {
                _currentHoverGizmo.OnDragCompleted(this);
                _state = States.Default;
            }
            else if (_state == States.PointerCapturedOnClickable)
            {
                _capturedClickTarget.PointerUntriggered(_laserPointer);
                _capturedClickTarget = null;
                _state = States.Default;
                _laserPointer.IsLockedAtTarget = false;
            }
        }


        private void PadClickedHandler(object sender, ClickedEventArgs e)
        {
            if (_laserPointer.PointingAt is TeleportationSpot)
            {
                _teleportation.JumpToSpot(_laserPointer.PointingAt as TeleportationSpot);
                if (_audioSourceForTeleportation)
                    _audioSourceForTeleportation.Play();
            }
            else if (_laserPointer.PointingAt is TeleportationZone)
            {
                _teleportation.JumpToPosition(_laserPointer.LastHitPoint);
                if (_audioSourceForTeleportation)
                    _audioSourceForTeleportation.Play();
            }
            else if (_laserPointer.PointingAt is AnnotationPositionTeleporter)
            {
                _capturedAnnotationTeleporter = _laserPointer.PointingAt as AnnotationPositionTeleporter;
                _capturedAnnotationTeleporter.OnClick(_teleportation);
                _capturedAnnotationTeleporter.SetPerspectiveHighlight(true);
                _state = States.PointerCapturedOnTeleporter;
            }
        }


        private void PadUnclickedHandler(object sender, ClickedEventArgs e)
        {
            if (_state == States.PointerCapturedOnTeleporter)
            {
                _capturedAnnotationTeleporter.OnUnclick(_teleportation);
                _capturedAnnotationTeleporter.SetPerspectiveHighlight(false);
                _capturedAnnotationTeleporter = null;
                _state = States.Default;
            }
        }

        private void ControllerGrippedHandler(object sender, ClickedEventArgs e)
        {
            if (_state == States.Default)
                AnnotationManager._instance.GoToNextAnnotation(_teleportation);
        }

        InteractiveGizmo GetGizmoUnderController()
        {
            InteractiveGizmo newHoverGizmo = null;
            var position = transform.position;
            foreach (var collider in GizmoColliders)
            {
                if (collider.bounds.Contains(position))
                {
                    newHoverGizmo = collider.gameObject.GetComponent<InteractiveGizmo>();
                    Debug.Log(">>>> collider under controller:" + collider.gameObject, this);
                    break;
                }
            }

            return newHoverGizmo;
        }

        // ToDo: will be used later
        // void SendPositionToGizmos()
        // {
        //     var position = transform.position;
        //     foreach (var collider in GizmoColliders)
        //     {
        //         if (!collider.gameObject.activeSelf)
        //             continue;

        //         var newHoverGizmo = collider.gameObject.GetComponent<InteractiveGizmo>();
        //         newHoverGizmo.OnControllerPositionUpdate(this);
        //     }
        // }

        public void TriggerHapticPulse(ushort durationMicroSec = 500)
        {
            var controllerInput = SteamVR_Controller.Input((int)_controller.controllerIndex);
            controllerInput.TriggerHapticPulse(durationMicroSec);
        }

        private Teleportation _teleportation;
        protected LaserPointer _laserPointer;
        private GameObject _laserInstance;
        protected SteamVR_TrackedController _controller;
        private InteractiveGizmo _currentHoverGizmo;
        private IClickableLaserPointerTarget _capturedClickTarget;
        private AnnotationPositionTeleporter _capturedAnnotationTeleporter;

        private States _state = States.Default;

        public States ActiveState
        {
            get
            {
                return _state;
            }
            private set
            {
                if (value == _state)
                {
                    Debug.LogWarning("Setting state " + value + " again?");
                    return;
                }
                _state = value;
            }
        }

        public enum States
        {
            Default = 0,
            IsCollidingWithGizmo,
            DraggingGizmo,
            PointerCapturedOnClickable,
            PointerCapturedOnTeleporter
        }
    }
}
