using ff.vr.annotate;
using UnityEngine;


namespace ff.vr.interaction
{

    public class AnnotationPositionTeleporter : LaserPointerButton
    {
        public Vector3 GetTeleportationTarget()
        {
            var teleportationTarget = _annotationGizmo.Annotation.ViewPointPosition.position;
            return new Vector3(teleportationTarget.x, 0, teleportationTarget.z);
        }

        void Start()
        {
            _annotationGizmo = GetComponentInParent<AnnotationGizmo>();
        }

        public void ShowPerspective()
        {
            var visualization = PerspectiveVisualization.Instance;
            visualization.transform.position =
            new Vector3(_annotationGizmo.Annotation.ViewPointPosition.position.x, 0f, _annotationGizmo.Annotation.ViewPointPosition.position.z);
            visualization.Show();
        }

        public void HidePerspective()
        {
            PerspectiveVisualization.Instance.Hide();
        }

        public void SetPerspectiveHighlight(bool highlighted)
        {
            PerspectiveVisualization.Instance.SetHighlight(highlighted);
        }

        private AnnotationGizmo _annotationGizmo;

    }
}