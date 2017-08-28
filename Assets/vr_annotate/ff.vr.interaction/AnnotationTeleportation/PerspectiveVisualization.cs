using System.Collections;
using System.Collections.Generic;
using ff.utils;
using UnityEngine;

namespace ff.vr.interaction
{
    public class PerspectiveVisualization : Singleton<PerspectiveVisualization>
    {
        [SerializeField]
        private GameObject Target;

        [SerializeField]
        private GameObject TargetArrow;

        void Start()
        {
            _inverseScale = this.transform.InverseTransformVector(Vector3.one);
            _allRenderers = GetComponentsInChildren<Renderer>();
        }

        void Update()
        {
            _hoverDamped = Mathf.Lerp(_hoverDamped, _hover, 0.1f);
            var isVisible = _hoverDamped > 0.001f;

            if (isVisible)
            {
                Target.transform.localScale = _inverseScale * _hoverDamped * HIGHLIGHT_SIZE;


                TargetArrow.transform.localPosition = _highlighted
                ? new Vector3(0, Mathf.Pow(Mathf.Abs(Mathf.Sin(Time.time * 5)), 0.7f) * 0.5f + 0.5f, 0)
                : new Vector3(0f, 1f, 0f);

                TargetArrow.transform.LookAt(Camera.main.transform.position);
                var rot = TargetArrow.transform.eulerAngles;
                TargetArrow.transform.eulerAngles = new Vector3(0, rot.y + 180, 0);

                foreach (var r in _allRenderers)
                {
                    var color = r.material.color;
                    r.material.color = new Color(color.r, color.g, color.b, _hoverDamped);
                }
            }
        }

        public void Show()
        {
            _hover = 1;
            Target.SetActive(true);
        }

        public void Hide()
        {
            _hover = 0;
            Target.SetActive(false);
        }

        public void SetHighlight(bool highlighted)
        {
            _highlighted = highlighted;
        }

        float _hover = 0;
        float _hoverDamped = 0;
        Vector3 _inverseScale = Vector3.one;
        private bool _isVisible;
        private bool _highlighted;
        private Renderer[] _allRenderers;

        const float HIGHLIGHT_SIZE = 1.0f;

    }
}