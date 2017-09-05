using System;
using ff.vr.annotate.viz;
using UnityEngine;
using UnityEngine.Events;

namespace ff.vr.interaction
{
    public class AnnotationGizmoButton : MonoBehaviour, IClickableLaserPointerTarget, ITeleportationTrigger
    {
        [Header("--- internal prefab references -----")]
        [SerializeField]
        private AnnotationPositionRenderer _annotationPositionRendererPrefab;
        [SerializeField]
        private OnTeleportationArrivalOrientation _onTeleportationArrivalOrientationPrefab;

        void Start()
        {
            _annotationGizmo = GetComponentInParent<AnnotationGizmo>();
        }

        void OnEnable()
        {
            SelectionManager.Instance.OnAnnotationGizmoHover += OnAnnotationGizmoHoverHandler;
        }

        void OnDisable()
        {
            SelectionManager.Instance.OnAnnotationGizmoHover -= OnAnnotationGizmoHoverHandler;
        }

        private void OnAnnotationGizmoHoverHandler(AnnotationGizmo obj)
        {
            if (_spawnedRenderer == null && obj == _annotationGizmo)
            {
                SpawnAnnotationPositionRenderer();
            }
        }

        public Vector3 GetTeleportationTarget()
        {
            var teleportationTarget = _annotationGizmo.Annotation.ViewPointPosition.position;
            return new Vector3(teleportationTarget.x, 0, teleportationTarget.z);
        }

        private void SpawnAnnotationPositionRenderer()
        {
            _spawnedRenderer = Instantiate(_annotationPositionRendererPrefab);
            _spawnedRenderer.SetAnnotationData(_annotationGizmo);
        }

        public void PointerEnter(LaserPointer pointer)
        {
            SelectionManager.Instance.SetOnAnnotationGizmoHover(_annotationGizmo);
        }

        public void PointerExit(LaserPointer pointer)
        {
            SelectionManager.Instance.SetOnAnnotationGizmoUnhover(_annotationGizmo);
        }

        public void PointerUntriggered(LaserPointer pointer)
        {
            SelectionManager.Instance.SetSelectedItem(_annotationGizmo);
        }

        public void PointerTriggered(LaserPointer pointer)
        {
        }

        public void PointerUpdate(LaserPointer pointer)
        {
        }

        public void PadClicked(Teleportation teleportation)
        {
            var arrivalHelp = Instantiate(_onTeleportationArrivalOrientationPrefab);
            arrivalHelp.SetAnnotationData(_annotationGizmo.Annotation);
            teleportation.JumpToPosition(GetTeleportationTarget());
            SelectionManager.Instance.SetSelectedItem(_annotationGizmo);
        }

        public void PadUnclicked(Teleportation teleportation)
        {
        }

        public AnnotationPositionRenderer AnnotationPositionRenderer
        {
            get
            {
                if (_spawnedRenderer == null)
                    SpawnAnnotationPositionRenderer();
                return _spawnedRenderer;
            }
        }

        private AnnotationGizmo _annotationGizmo;
        private AnnotationPositionRenderer _spawnedRenderer;
    }
}