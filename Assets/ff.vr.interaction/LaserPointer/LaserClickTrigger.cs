using UnityEngine;
using UnityEngine.Events;

namespace ff.vr.interaction
{
    [RequireComponent(typeof(Collider))]
    public class LaserClickTrigger : MonoBehaviour, IClickableLaserPointerTarget
    {
        public UnityEvent OnLaserEnter;
        public UnityEvent OnLaserExit;
        public UnityEvent OnLaserClick;

        // Implement interface
        public void PointerEnter(LaserPointer pointer)
        {
            OnLaserEnter.Invoke();
        }
        public void PointerExit(LaserPointer pointer)
        {
            OnLaserExit.Invoke();

        }
        public void PointerUpdate(LaserPointer pointer)
        {

        }

        public void PointerTriggered(LaserPointer pointer)
        {
            OnLaserClick.Invoke();
        }

        public void PointerUntriggered(LaserPointer pointer)
        {

        }
    }
}