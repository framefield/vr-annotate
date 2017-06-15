using UnityEngine;
using ff.vr.annotate;
using ff.nodegraph;

namespace ff.vr.annotate
{
    public class AnnotationGizmo : MonoBehaviour
    {
        public Annotation Annotation;

        [Header("--- internal prefab references-----")]
        public TMPro.TextMeshPro LabelText;

        public AnimationCurve ScaleUpOverTime;

        void Update()
        {
            if (_lastAnnotation != Annotation)
            {
                if (Annotation == null)
                {
                    LabelText.text = "";
                }
                else if (Annotation.ContextNode != null)
                {
                    _startTime = Time.time;
                    LabelText.text = Annotation.ContextNode.Name;
                }
                _lastAnnotation = Annotation;
            }

            var s = ScaleUpOverTime.Evaluate(Time.time - _startTime);
            this.transform.localScale = new Vector3(s, s, s);

            // Face towards camera
            LabelText.transform.LookAt(LabelText.transform.position - Camera.main.transform.position + LabelText.transform.position);
        }

        private Annotation _lastAnnotation;
        private float _startTime;
    }


}
