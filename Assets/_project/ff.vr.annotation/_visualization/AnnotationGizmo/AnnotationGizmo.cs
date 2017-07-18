using UnityEngine;
using ff.vr.annotate;
using ff.nodegraph;
using ff.vr.interaction;
using System;

namespace ff.vr.annotate
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
        LaserPointerButton _icon;


        [SerializeField]
        GameObject _hoverGroup;

        public Color SelectedColor = Color.white;
        public Color Color = Color.gray;

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



        private void UpdateUI()
        {
            if (_annotation != null)
            {
                _annotationObjectLabel.text = _annotation.TargetNode != null ? _annotation.TargetNode.Name : "<Without Object>"; // FIXME: Needs to be implemented
                _annotationBodyLabel.text = _annotation.Text;
                _authorLabel.text = _annotation.Author.name;
                _annotationDateLabel.text = _annotation.CreatedAt.ToString("yyyy/MM/dd");
            }
            _icon.SetColor(_isSelected ? SelectedColor : Color);
            _hoverGroup.SetActive(_isHovered && !_isSelected);
        }

        public void UpdateBodyText(string newText)
        {
            _annotationBodyLabel.text = newText;
        }


        public void OnHover()
        {
            _isHovered = true;
            UpdateUI();
        }

        public void OnUnhover()
        {
            _isHovered = false;
            UpdateUI();
        }

        public void OnClicked()
        {
            SelectionManager.Instance.SelectItem(this);
        }
        #region implemented ISelectable


        public Vector3 GetPosition()
        {
            return transform.position;
        }
        #endregion

        private bool _isHovered = false;
        private float _startTime;
        private bool _isSelected = false;
        private Annotation _annotation;
        private const float DEFAULT_SIZE = 0.3f;
    }
}
