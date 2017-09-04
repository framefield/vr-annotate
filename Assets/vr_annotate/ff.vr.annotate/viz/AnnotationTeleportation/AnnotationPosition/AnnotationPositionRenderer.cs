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
        private bool IsPhantom;
        private float PhantomFactor;

        [Header("--- configuration -----")]

        [SerializeField]
        private Color HoverColor;
        [SerializeField]
        private Color SelectedColor;
        [SerializeField]
        private Color DefaultColor;

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
        [SerializeField]
        private GameObject _silhouette;
        [SerializeField]



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

            Selected = (annotationGizmo != null) && (annotationGizmo == _renderedAnnotationGizmo);
        }

        private void OnAnnotationGizmoUnhoverHandler(AnnotationGizmo annotationGizmo)
        {
            if (_renderedAnnotationGizmo != annotationGizmo)
                return;
            Hovered = false;
        }

        private void OnAnnotationGizmoHoverHandler(AnnotationGizmo annotationGizmo)
        {
            if (_renderedAnnotationGizmo != annotationGizmo)
                return;
            Hovered = true;
        }

        void Update()
        {
            if (Hovered || Selected || IsPhantom)
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
            foreach (var r in _allRenderers)
                r.enabled = true;

            _target.transform.localScale = _initialScale * (MINSIZE + _visibility * (1 - MINSIZE));

            _arrow.transform.localPosition =
            Highlighted
            ? new Vector3(0, Mathf.Pow(Mathf.Abs(Mathf.Sin(Time.time * 5)), 0.7f) * 0.5f, 0)
            : new Vector3(0f, 0f, 0f);

            if (Hovered || Selected)
            {
                _silhouette.SetActive(true);
                _silhouette.transform.LookAt(Camera.main.transform.position);
                _silhouette.transform.eulerAngles = new Vector3(0, _arrow.transform.eulerAngles.y + 180, 0);
                _arrow.SetActive(true);
                _arrow.transform.LookAt(Camera.main.transform.position);
                _arrow.transform.eulerAngles = new Vector3(0, _arrow.transform.eulerAngles.y + 180, 0);
            }
            else
            {
                _silhouette.SetActive(false);
                _arrow.SetActive(false);
            }

            foreach (var r in _allRenderers)
            {
                Color colorWithAlpha;

                if (Hovered)
                {
                    colorWithAlpha = new Color(HoverColor.r, HoverColor.g, HoverColor.b, AlphaOverVisibility.Evaluate(_visibility));
                }
                else if (Selected)
                {
                    colorWithAlpha = new Color(SelectedColor.r, SelectedColor.g, SelectedColor.b, AlphaOverVisibility.Evaluate(_visibility));
                }
                else if (IsPhantom)
                {
                    colorWithAlpha = new Color(DefaultColor.r, DefaultColor.g, DefaultColor.b, AlphaOverVisibility.Evaluate(_visibility * PhantomFactor / 4f));
                    // Debug.Log("PhantomFactor:" + PhantomFactor);
                }
                else
                {
                    // render last color with current alpha
                    var lastColor = r.material.color;
                    colorWithAlpha = new Color(lastColor.r, lastColor.g, lastColor.b, AlphaOverVisibility.Evaluate(_visibility));
                }

                r.material.SetColor("_Color", colorWithAlpha);
                r.material.SetColor("_EmissionColor", colorWithAlpha);
            }
            if (Hovered)
            {
                _lineToAnnotation.gameObject.SetActive(true);
                _lineToAnnotation.SetPosition(1, _lineEndPosition + LineLengthOverVisibility.Evaluate(_visibility) * (_lineStartPosition - _lineEndPosition));
                _lineToAnnotation.widthMultiplier = LineWidthOverVisibility.Evaluate(_visibility);
            }
            else
            {
                _lineToAnnotation.gameObject.SetActive(false);
            }
        }

        public void SetPhantom(float factor)
        {
            IsPhantom = factor > 0f;
            PhantomFactor = factor;
        }

        // todo should be Constructor
        public void SetAnnotationData(AnnotationGizmo annotationGizmo)
        {
            _renderedAnnotationGizmo = annotationGizmo;
            var annotation = _renderedAnnotationGizmo.Annotation;
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
        public AnnotationGizmo _renderedAnnotationGizmo;

    }
}