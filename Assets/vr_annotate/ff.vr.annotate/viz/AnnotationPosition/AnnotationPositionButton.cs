using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ff.vr.interaction;

namespace ff.vr.annotate.viz
{
    [RequireComponent(typeof(AnnotationPositionRenderer))]
    public class AnnotationPositionButton : MonoBehaviour, IClickableLaserPointerTarget, ITeleportationTrigger
    {
        [SerializeField]
        private OnTeleportationArrivalOrientation _onTeleportationArrivalOrientationPrefab;

        void Start()
        {
            _gizmo = GetComponent<AnnotationPositionRenderer>().RenderedAnnotationGizmo;
        }

        void ITeleportationTrigger.PadClicked(Teleportation teleportation)
        {
            var arrivalHelp = Instantiate(_onTeleportationArrivalOrientationPrefab);
            arrivalHelp.SetAnnotationData(_gizmo.Annotation);
            teleportation.JumpToPosition(GetTeleportationTarget(_gizmo));
            SelectionManager.Instance.SetSelectedItem(_gizmo);
        }

        void ITeleportationTrigger.PadUnclicked(Teleportation teleportation)
        {
        }

        void ILaserPointerTarget.PointerEnter(LaserPointer pointer)
        {
            SelectionManager.Instance.SetOnAnnotationGizmoHover(_gizmo);
        }

        void ILaserPointerTarget.PointerExit(LaserPointer pointer)
        {
            SelectionManager.Instance.SetOnAnnotationGizmoUnhover(_gizmo);
        }

        void IClickableLaserPointerTarget.PointerTriggered(LaserPointer pointer)
        {
            SelectionManager.Instance.SetSelectedItem(_gizmo);
        }

        void IClickableLaserPointerTarget.PointerUntriggered(LaserPointer pointer)
        {
        }

        void ILaserPointerTarget.PointerUpdate(LaserPointer pointer)
        {
        }

        public Vector3 GetTeleportationTarget(AnnotationGizmo gizmo)
        {
            var teleportationTarget = gizmo.Annotation.ViewPointPosition.position;
            return new Vector3(teleportationTarget.x, 0, teleportationTarget.z);
        }

        private AnnotationGizmo _gizmo;
    }
}