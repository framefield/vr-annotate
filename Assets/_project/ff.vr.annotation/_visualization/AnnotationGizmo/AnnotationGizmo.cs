using UnityEngine;
using ff.vr.annotate;
using ff.nodegraph;

namespace ff.vr.annotate
{
    public class AnnotationGizmo : MonoBehaviour
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

        void Update()
        {
            utils.Helpers.FaceCameraAndKeepSize(this.transform, DEFAULT_SIZE);
        }

        /** Called from annotation manager */
        public void SetAnnotation(Annotation newAnnotation)
        {
            _annotation = newAnnotation;
            UpdateVisibility();
        }

        private void UpdateVisibility()
        {
            _annotationObjectLabel.text = _annotation.TargetNode != null ? _annotation.TargetNode.Name : "<Without Object>"; // FIXME: Needs to be implemented
            _annotationBodyLabel.text = _annotation.Text;
            _authorLabel.text = _annotation.Author.name;
            _annotationDateLabel.text = _annotation.CreatedAt.ToString("yyyy/MM/dd");
        }

        public void UpdateBodyText(string newText)
        {
            _annotationBodyLabel.text = newText;
        }


        public void OnHover()
        {
            _isHovered = true;
            UpdateVisibility();
        }

        public void OnUnhover()
        {
            _isHovered = false;
            UpdateVisibility();
        }

        private bool _isHovered = false;
        private float _startTime;
        private Annotation _annotation;
        private const float DEFAULT_SIZE = 0.3f;
    }
}
