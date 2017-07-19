using UnityEngine;

namespace ff.vr.interaction
{
    public class TeleportationZone : MonoBehaviour, ILaserPointerTarget, IHoverColor
    {
        public Color HoverColor;

        public GameObject Hightlight;
        public GameObject TargetArrow;

        void Start()
        {
            _inverseScale = this.transform.InverseTransformVector(Vector3.one);
            _mainCam = GameObject.FindGameObjectWithTag("MainCamera");
            _telepotationZoneRenderer = this.gameObject.GetComponent<Renderer>();
        }

        void Update()
        {
            if (!Hightlight)
                throw new UnityException("Teleportation TargetZone is missing Highlight-Object reference");

            _hoverDamped = Mathf.Lerp(_hoverDamped, _hover, 0.1f);
            var isVisible = _hoverDamped > 0.001f;


            if (isVisible)
            {
                Hightlight.transform.localScale = _inverseScale * _hoverDamped * HIGHLIGHT_SIZE;
                TargetArrow.transform.localPosition = new Vector3(0, Mathf.Pow(Mathf.Abs(Mathf.Sin(Time.time * 5)), 0.7f) * 0.5f + 0.5f, 0);
                TargetArrow.transform.LookAt(_mainCam.transform.position);
                var rot = TargetArrow.transform.eulerAngles;
                TargetArrow.transform.eulerAngles = new Vector3(0, rot.y + 180, 0);

                _telepotationZoneRenderer.material.color = new Color(0, 0, 1, _hoverDamped);
            }
        }

        public void PointerEnter(LaserPointer pointer)
        {
            _hover = 1;
            Hightlight.SetActive(true);
        }

        public void PointerUpdate(LaserPointer pointer)
        {
            Hightlight.transform.position = pointer.LastHitPoint;
        }

        public void PointerExit(LaserPointer pointer)
        {
            _hover = 0;
            Hightlight.SetActive(false);
        }


        public Color GetHoverColor()
        {
            return HoverColor;
        }


        float _hover = 0;
        float _hoverDamped = 0;

        Renderer _telepotationZoneRenderer;

        Vector3 _inverseScale = Vector3.one;
        GameObject _mainCam;

        const float HIGHLIGHT_SIZE = 0.3f;
    }
}