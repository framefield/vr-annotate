using System.Collections;
using System.Collections.Generic;
using ff.utils;
using ff.vr.annotate.datamodel;
using UnityEngine;

namespace ff.vr.interaction
{
    public class AnnotationPositionRenderer : Singleton<AnnotationPositionRenderer>
    {

        [SerializeField]
        private LineRenderer _lineToAnnotation;

        [SerializeField]
        private GameObject _target;

        [SerializeField]
        private GameObject _arrow;

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
                _target.transform.localScale = _inverseScale * _hoverDamped * HIGHLIGHT_SIZE;

                _arrow.transform.localPosition =
                _highlighted
                ? new Vector3(0, Mathf.Pow(Mathf.Abs(Mathf.Sin(Time.time * 5)), 0.7f) * 0.5f, 0)
                : new Vector3(0f, 0f, 0f);

                _arrow.transform.LookAt(Camera.main.transform.position);
                var rot = _arrow.transform.eulerAngles;
                _arrow.transform.eulerAngles = new Vector3(0, rot.y + 180, 0);

                foreach (var r in _allRenderers)
                {
                    var color = r.material.color;
                    r.material.color = new Color(color.r, color.g, color.b, _hoverDamped);
                }
            }
        }

        public void RenderAnnotation(Annotation annotation)
        {
            transform.position = new Vector3(annotation.ViewPointPosition.position.x, 0, annotation.ViewPointPosition.position.z);

            var startPos = annotation.ViewPointPosition.position;
            Debug.Log(startPos);
            var endPos = annotation.AnnotationPosition.position;

            _lineToAnnotation.SetPosition(0, startPos);
            _lineToAnnotation.SetPosition(1, (startPos + endPos) / 2);
            _lineToAnnotation.SetPosition(2, endPos);

            Show();
        }

        public void Show()
        {
            _hover = 1;
            _target.SetActive(true);
        }

        public void Hide()
        {
            _hover = 0;
            _target.SetActive(false);
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