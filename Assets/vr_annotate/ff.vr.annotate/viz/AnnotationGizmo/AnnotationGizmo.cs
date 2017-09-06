using UnityEngine;
using ff.vr.annotate;
using ff.nodegraph;
using ff.vr.interaction;
using System;
using ff.vr.annotate.datamodel;

namespace ff.vr.annotate.viz
{
    public class AnnotationGizmo : MonoBehaviour, ISelectable
    {

        [Header("--- internal prefab references-----------")]
        [SerializeField]
        TMPro.TextMeshPro _annotationObjectLabel;
        [SerializeField]
        TMPro.TextMeshPro _annotationBodyLabel;
        [SerializeField]
        TMPro.TextMeshPro _authorLabel;
        [SerializeField]
        TMPro.TextMeshPro _annotationDateLabel;
        [SerializeField]
        TMPro.TextMeshPro _tourPositionLabel;

        [SerializeField]
        Renderer _icon;


        [SerializeField]
        GameObject _hoverGroup;
        [SerializeField]
        GameObject _tourGroup;

        public Color SelectedColor;
        public Color HoveredColor;
        public Color Color;

        void Update()
        {
            utils.Helpers.FaceCameraAndKeepSize(this.transform, DEFAULT_SIZE);
        }

        /** Called from annotation manager */
        public Annotation Annotation
        {
            get { return _annotation; }
            set
            {
                _annotation = value;
                UpdateUI();
            }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                UpdateUI();
            }
        }

        void OnEnable()
        {
            SelectionManager.Instance.OnSelectedAnnotationGizmoChanged += SelectedAnnotationGizmoChangedHandler;
            SelectionManager.Instance.OnAnnotationGizmoHover += OnAnnotationGizmoHoverHandler;
            SelectionManager.Instance.OnAnnotationGizmoUnhover += OnAnnotationGizmoUnhoverHandler;
        }

        void OnDisable()
        {
            SelectionManager.Instance.OnSelectedAnnotationGizmoChanged -= SelectedAnnotationGizmoChangedHandler;
            SelectionManager.Instance.OnAnnotationGizmoHover -= OnAnnotationGizmoHoverHandler;
            SelectionManager.Instance.OnAnnotationGizmoUnhover -= OnAnnotationGizmoUnhoverHandler;
        }


        private void OnAnnotationGizmoHoverHandler(AnnotationGizmo obj)
        {
            if (obj == this)
            {
                _isHovered = true;
                UpdateUI();
            }
        }

        private void OnAnnotationGizmoUnhoverHandler(AnnotationGizmo obj)
        {
            if (obj == this)
            {
                _isHovered = false;
                UpdateUI();
            }
        }

        private void SelectedAnnotationGizmoChangedHandler(AnnotationGizmo obj)
        {
            _isSelected = (obj == this);
            UpdateUI();
        }

        public void OnHover()
        {
            SelectionManager.Instance.SetOnAnnotationGizmoHover(this);
        }

        public void OnUnhover()
        {
            SelectionManager.Instance.SetOnAnnotationGizmoUnhover(this);
        }

        public void OnClicked()
        {
            SelectionManager.Instance.SetSelectedItem(this);
        }

        private void UpdateUI()
        {
            if (_annotation != null)
            {
                _annotationObjectLabel.text = _annotation.TargetNode != null ? _annotation.TargetNode.Name : "<Without Object>";
                _annotationBodyLabel.text = _annotation.Text;
                _authorLabel.text = _annotation.Author.name;
                _annotationDateLabel.text = _annotation.CreatedAt.ToString("yyyy/MM/dd");
                // _tourPositionLabel.text = AnnotationTour.Instance.GetIndexOfGizmoInList(this) + " / " + AnnotationTour.Instance.GetLengthOfTour();
            }

            if (_isSelected)
                _icon.material.SetColor("_tintColor", SelectedColor);
            else if (_isHovered)
                _icon.material.SetColor("_tintColor", HoveredColor);
            else
                _icon.material.SetColor("_tintColor", Color);

            _hoverGroup.SetActive(_isHovered | _isSelected);
            _tourGroup.SetActive(_isSelected);
        }

        public void UpdateBodyText(string newText)
        {
            _annotationBodyLabel.text = newText;
        }
        #region implemented ISelectable


        public Vector3 GetPosition()
        {
            return transform.position;
        }
        #endregion

        public bool _isHovered = false;
        public bool _isSelected = false;
        private float _startTime;
        private Annotation _annotation;
        private const float DEFAULT_SIZE = 0.3f;
    }
}
