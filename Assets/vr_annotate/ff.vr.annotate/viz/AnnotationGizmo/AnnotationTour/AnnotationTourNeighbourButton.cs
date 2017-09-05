using System.Collections;
using System.Collections.Generic;
using ff.vr.interaction;
using UnityEngine;

namespace ff.vr.annotate.viz
{
    public class AnnotationTourNeighbourButton : MonoBehaviour, IClickableLaserPointerTarget, ITeleportationTrigger
    {
        [SerializeField]
        private OnTeleportationArrivalOrientation _onTeleportationArrivalOrientationPrefab;
        [SerializeField]
        private AnnotationGizmo _attachedGizmo;

        public bool InverseDirection;

        void ITeleportationTrigger.PadClicked(Teleportation teleportation)
        {
            var arrivalHelp = Instantiate(_onTeleportationArrivalOrientationPrefab);
            arrivalHelp.SetAnnotationData(GetTargetGizmo().Annotation);
            teleportation.JumpToPosition(GetTeleportationTarget(GetTargetGizmo()));
        }

        void ITeleportationTrigger.PadUnclicked(Teleportation teleportation)
        {
        }

        void ILaserPointerTarget.PointerEnter(LaserPointer pointer)
        {
            SelectionManager.Instance.SetOnAnnotationGizmoHover(GetTargetGizmo());
        }

        void ILaserPointerTarget.PointerExit(LaserPointer pointer)
        {
            SelectionManager.Instance.SetOnAnnotationGizmoUnhover(GetTargetGizmo());
        }

        void IClickableLaserPointerTarget.PointerTriggered(LaserPointer pointer)
        {
            SelectionManager.Instance.SetSelectedItem(GetTargetGizmo());
        }

        void IClickableLaserPointerTarget.PointerUntriggered(LaserPointer pointer)
        {
        }

        void ILaserPointerTarget.PointerUpdate(LaserPointer pointer)
        {
        }

        public AnnotationGizmo GetTargetGizmo()
        {
            return
            InverseDirection ?
            AnnotationTour.Instance.GetPreviousGizmo(_attachedGizmo) :
            AnnotationTour.Instance.GetNextGizmo(_attachedGizmo);
        }

        public Vector3 GetTeleportationTarget(AnnotationGizmo gizmo)
        {
            var teleportationTarget = gizmo.Annotation.ViewPointPosition.position;
            return new Vector3(teleportationTarget.x, 0, teleportationTarget.z);
        }

    }

}