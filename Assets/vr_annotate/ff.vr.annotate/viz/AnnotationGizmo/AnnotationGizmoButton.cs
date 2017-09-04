using System;
using ff.vr.annotate;
using UnityEngine;
using UnityEngine.Events;

namespace ff.vr.interaction
{
    public class AnnotationGizmoButton : LaserPointerButton, ITeleportationTrigger
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

        public Vector3 GetTeleportationTarget()
        {
            var teleportationTarget = _annotationGizmo.Annotation.ViewPointPosition.position;
            return new Vector3(teleportationTarget.x, 0, teleportationTarget.z);
        }

        public override void PointerEnter(LaserPointer pointer)
        {
            base.PointerEnter(pointer);
            if (_spawnedRenderer == null)
            {
                _spawnedRenderer = Instantiate(_annotationPositionRendererPrefab);
                _spawnedRenderer.SetAnnotationData(_annotationGizmo.Annotation);
            }
            SelectionManager.Instance.SetOnAnnotationGizmoHover(_annotationGizmo);
        }

        private void SpawnAnnotationPositionRenderer()
        {
        }

        public override void PointerExit(LaserPointer pointer)
        {
            SelectionManager.Instance.SetOnAnnotationGizmoUnhover(GetComponentInParent<AnnotationGizmo>());
            base.PointerExit(pointer);
        }

        public override void PointerUntriggered(LaserPointer pointer)
        {
            SelectionManager.Instance.SetSelectedItem(GetComponentInParent<AnnotationGizmo>());
            base.PointerUntriggered(pointer);
        }

        public void PadClicked(Teleportation teleportation)
        {
        }

        public void PadUnclicked(Teleportation teleportation)
        {
            var arrivalHelp = Instantiate(_onTeleportationArrivalOrientationPrefab);
            arrivalHelp.SetAnnotationData(_annotationGizmo.Annotation);
            teleportation.JumpToPosition(GetTeleportationTarget());
        }

        private AnnotationGizmo _annotationGizmo;
        private AnnotationPositionRenderer _spawnedRenderer;
    }
}