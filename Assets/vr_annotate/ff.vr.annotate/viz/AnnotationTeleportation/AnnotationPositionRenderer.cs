using System;
using System.Collections;
using System.Collections.Generic;
using ff.utils;
using ff.vr.annotate;
using ff.vr.annotate.datamodel;
using UnityEngine;

namespace ff.vr.interaction
{
    public class AnnotationPositionRenderer : MonoBehaviour
    {
        private bool Highlighted;
        private bool Selected;
        private bool Hovered;

        [Header("--- configuration -----")]

        [SerializeField]
        private Color HoverColor;
        [SerializeField]
        private Color SelectedColor;

        [SerializeField]
        private AnimationCurve AlphaOverVisibility;
        [SerializeField]
        private AnimationCurve LineLengthOverVisibility;
        [SerializeField]
        private AnimationCurve LineWidthOverVisibility;
        [SerializeField]
        private float LinePaddingOnEndPoints;


        [Header("--- internal prefab references -----")]

        [SerializeField]
        private LineRenderer _lineToAnnotation;
        [SerializeField]
        private GameObject _target;
        [SerializeField]
        private GameObject _arrow;

        void Start()
        {
            _initialScale = this.transform.localScale;
            _allRenderers = GetComponentsInChildren<Renderer>();
        }

        void OnEnable()
        {
            SelectionManager.Instance.SelectedAnnotationGizmoChangedEvent += SelectedAnnotationGizmoChangedHandler;
            SelectionManager.Instance.OnAnnotationGizmoHover += OnAnnotationGizmoHoverHandler;
            SelectionManager.Instance.OnAnnotationGizmoUnhover += OnAnnotationGizmoUnhoverHandler;
        }

        void OnDisable()
        {
            SelectionManager.Instance.SelectedAnnotationGizmoChangedEvent -= SelectedAnnotationGizmoChangedHandler;
            SelectionManager.Instance.OnAnnotationGizmoHover -= OnAnnotationGizmoHoverHandler;
            SelectionManager.Instance.OnAnnotationGizmoUnhover -= OnAnnotationGizmoUnhoverHandler;
        }

        private void SelectedAnnotationGizmoChangedHandler(AnnotationGizmo annotationGizmo)
        {

            Selected = (annotationGizmo != null) && (annotationGizmo.Annotation == _renderedAnnotation);
        }

        private void OnAnnotationGizmoUnhoverHandler(AnnotationGizmo annotationGizmo)
        {
            if (_renderedAnnotation != annotationGizmo.Annotation)
                return;
            Hovered = false;
        }

        private void OnAnnotationGizmoHoverHandler(AnnotationGizmo annotationGizmo)
        {
            if (_renderedAnnotation != annotationGizmo.Annotation)
                return;
            Hovered = true;
        }

        void Update()
        {
            if (Hovered || Selected)
                _shouldHaveVisibility = 1;
            else
                _shouldHaveVisibility = 0;

            _visibility = Mathf.Lerp(_visibility, _shouldHaveVisibility, 0.2f);
            var isVisible = _visibility > 0.001f;

            if (isVisible)
                RenderAnnotationPositionForCurrentVisibility();
            else
                GameObject.Destroy(gameObject);
        }


        private void RenderAnnotationPositionForCurrentVisibility()
        {
            _target.transform.localScale = _initialScale * (MINSIZE + _visibility * (1 - MINSIZE));

            _arrow.transform.localPosition =
            Highlighted
            ? new Vector3(0, Mathf.Pow(Mathf.Abs(Mathf.Sin(Time.time * 5)), 0.7f) * 0.5f, 0)
            : new Vector3(0f, 0f, 0f);

            _arrow.transform.LookAt(Camera.main.transform.position);
            _arrow.transform.eulerAngles = new Vector3(0, _arrow.transform.eulerAngles.y + 180, 0);

            foreach (var r in _allRenderers)
            {
                var colorToSet = Selected ? SelectedColor : HoverColor;
                var colorWithAlpha = new Color(colorToSet.r, colorToSet.g, colorToSet.b, AlphaOverVisibility.Evaluate(_visibility));

                r.material.SetColor("_Color", colorWithAlpha);
                r.material.SetColor("_EmissionColor", colorWithAlpha);
            }

            _lineToAnnotation.SetPosition(1, _lineEndPosition + LineLengthOverVisibility.Evaluate(_visibility) * (_lineStartPosition - _lineEndPosition));
            _lineToAnnotation.widthMultiplier = LineWidthOverVisibility.Evaluate(_visibility);
        }

        // todo should be Constructor
        public void SetAnnotationData(Annotation annotation)
        {
            _renderedAnnotation = annotation;
            transform.position = new Vector3(annotation.ViewPointPosition.position.x, 0, annotation.ViewPointPosition.position.z);

            _lineStartPosition = annotation.ViewPointPosition.position;
            _lineEndPosition = annotation.AnnotationPosition.position;

            var startToEnd = (_lineEndPosition - _lineStartPosition);
            if (startToEnd.magnitude > 2 * LinePaddingOnEndPoints)
            {
                _lineStartPosition += startToEnd.normalized * LinePaddingOnEndPoints;
                _lineEndPosition -= startToEnd.normalized * LinePaddingOnEndPoints;
            }
            _lineToAnnotation.SetPosition(0, _lineEndPosition);
            _lineToAnnotation.SetPosition(1, _lineEndPosition);
        }

        public void Show()
        {
            _shouldHaveVisibility = 1;
        }

        public void Hide()
        {
            _shouldHaveVisibility = 0;
        }


        private float _shouldHaveVisibility = 0;
        private float _visibility = 0;

        Vector3 _initialScale = Vector3.one;

        private Renderer[] _allRenderers;

        private Vector3 _lineStartPosition;
        private Vector3 _lineEndPosition;

        private const float MINSIZE = 0.9f;
        private Annotation _renderedAnnotation;

    }
}