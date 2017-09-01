using ff.vr.annotate;
using UnityEngine;
using UnityEngine.Events;

namespace ff.vr.interaction
{
    public class AnnotationPositionTeleporter : LaserPointerButton, ITeleportationTrigger
    {

        void Start()
        {
            _annotationGizmo = GetComponentInParent<AnnotationGizmo>();
        }

        public Vector3 GetTeleportationTarget()
        {
            var teleportationTarget = _annotationGizmo.Annotation.ViewPointPosition.position;
            return new Vector3(teleportationTarget.x, 0, teleportationTarget.z);
        }

        public void SetPerspectiveHighlight(bool highlighted)
        {
            AnnotationPositionRenderer.Instance.Highlighted = highlighted;
        }

        public override void PointerEnter(LaserPointer pointer)
        {
            base.PointerEnter(pointer);
            AnnotationPositionRenderer.Instance.SetAnnotationData(GetComponentInParent<AnnotationGizmo>().Annotation);
            AnnotationPositionRenderer.Instance.Hovered = true;
        }

        public override void PointerExit(LaserPointer pointer)
        {
            base.PointerExit(pointer);
            AnnotationPositionRenderer.Instance.Hovered = false;
        }

        public override void PointerUntriggered(LaserPointer pointer)
        {
            base.PointerUntriggered(pointer);
            AnnotationPositionRenderer.Instance.Selected = true;
        }

        public void PadClicked(Teleportation teleportation)
        {
            SetPerspectiveHighlight(true);
        }

        public void PadUnclicked(Teleportation teleportation)
        {
            teleportation.JumpToPosition(GetTeleportationTarget());
            SetPerspectiveHighlight(false);
        }

        private AnnotationGizmo _annotationGizmo;
    }
}