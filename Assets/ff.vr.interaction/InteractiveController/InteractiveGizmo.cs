using UnityEngine;

namespace ff.vr.interaction
{
    [RequireComponent(typeof(BoxCollider))]
    public class InteractiveGizmo : MonoBehaviour
    {
        virtual public void OnControllerEnter(InteractiveController controller) { }
        virtual public void OnControllerStay(InteractiveController controller) { }
        virtual public void OnControllerExit(InteractiveController controller) { }
        virtual public bool OnDragStarted(InteractiveController controller) { return false; }
        virtual public void OnDragUpdate(InteractiveController controller) { }
        virtual public void OnDragCompleted(InteractiveController controller) { }

        public void OnControllerPositionUpdate(InteractiveController controller) { }
        public void OnControllerEnabled(InteractiveController controller) { }
        public void OnControllerDisabled(InteractiveController controller) { }

        public void SetGizmoEnabled(bool enabled)
        {
            var collider = GetComponent<BoxCollider>();
            if (collider != null)
                collider.enabled = enabled;
            _isGizmoEnabled = enabled;
        }
        protected bool _isGizmoEnabled = false;
    }
}