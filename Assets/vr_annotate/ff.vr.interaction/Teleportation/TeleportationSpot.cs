using UnityEngine;


namespace ff.vr.interaction
{

    public class TeleportationSpot : MonoBehaviour, ILaserPointerTarget, IHoverColor
    {
        public Color HoverColor;

        public void Start()
        {
            _animator = gameObject.GetComponent<Animator>();
        }


        public void PointerEnter(LaserPointer pointer)
        {
            if (Vector3.Distance(transform.position, pointer.Controller.transform.position) > MIN_DISTANCE)
                SetActive(true);
        }


        public void PointerExit(LaserPointer pointer)
        {
            SetActive(false);
        }

        public void PointerUpdate(LaserPointer pointer)
        {

        }


        public void EnableTarget(bool enable)
        {
            gameObject.GetComponent<Collider>().enabled = enable;
            if (!enable)
                SetActive(enable);
        }


        public Color GetHoverColor()
        {
            return HoverColor;
        }


        private void SetActive(bool active)
        {
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(active);
            }
            _animator.SetBool("isLaserHit", active);
        }

        const float MIN_DISTANCE = 3;
        private Animator _animator;
    }
}