using System.Collections.Generic;
using UnityEngine;
using ff.vr.annotate;
using ff.nodegraph;
using ff.vr.interaction;
using System;

namespace ff.vr.annotate.viz
{
    /* 
        A singleton, that handles loading and adding annotations
        - Can be selected.
        - Can be hovered.
        - We be generated on the fly
        - Is grouped under a Annotations-Group
    */
    public class AnnotationManager : MonoBehaviour
    {


        private List<Annotation> AllAnnotations = new List<Annotation>();
        private List<AnnotationGizmo> AllAnnotationGizmos = new List<AnnotationGizmo>();

        public AnnotationGizmo _annotationGizmoPrefab;

        void Awake()
        {
            _gizmoContainer = new GameObject();
            _gizmoContainer.name = "GizmoContainer";
            _gizmoContainer.transform.SetParent(this.transform, false);
            _keyboardEnabler = FindObjectOfType<KeyboardEnabler>();
            _keyboardEnabler.Hide();
            _keyboardEnabler.InputCompleted += HandleInputCompleted;
            _keyboardEnabler.InputChanged += HandleInputChanged;
        }

        private void HandleInputCompleted()
        {
            _keyboardEnabler.Hide();
        }

        private void HandleInputChanged(string newText)
        {
            if (_focusedAnnotationGizmo)
                _focusedAnnotationGizmo.LabelText.text = newText;
        }


        public void CreateAnnotation(Node contextNode, Vector3 position)
        {
            var newAnnotation = new Annotation()
            {
                Position = position,
                ContextNodeId = contextNode.Id,
                ContextNode = contextNode,
                Id = System.Guid.NewGuid(),
            };

            AllAnnotations.Add(newAnnotation);

            var newGizmo = Instantiate(_annotationGizmoPrefab);
            newGizmo.transform.position = position;
            newGizmo.transform.SetParent(_gizmoContainer.transform, false);
            newGizmo.Annotation = newAnnotation;
            _focusedAnnotationGizmo = newGizmo;
            _keyboardEnabler.Show();
        }

        private AnnotationGizmo _focusedAnnotationGizmo;
        private GameObject _gizmoContainer;
        private KeyboardEnabler _keyboardEnabler;
    }
}
