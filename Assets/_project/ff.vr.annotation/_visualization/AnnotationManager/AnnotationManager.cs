using System.Collections.Generic;
using UnityEngine;
using ff.vr.annotate;
using ff.nodegraph;
using ff.vr.interaction;
using System;
using System.IO;
using ff.location;

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
        public string SimulatedYear = "300 BC";
        public string SimulatedTimeOfDay = "18:23:12";

        private List<Annotation> AllAnnotations = new List<Annotation>();
        private List<AnnotationGizmo> AllAnnotationGizmos = new List<AnnotationGizmo>();

        public AnnotationGizmo _annotationGizmoPrefab;

        public static AnnotationManager _instance;

        void Awake()
        {
            if (_instance != null)
            {
                Debug.LogError("Only one instance of " + this + " allowed", this);
            }
            _instance = this;

            _gizmoContainer = new GameObject();
            _gizmoContainer.name = "GizmoContainer";
            _gizmoContainer.transform.SetParent(this.transform, false);
            _keyboardEnabler = FindObjectOfType<KeyboardEnabler>();
            _keyboardEnabler.Hide();
            _keyboardEnabler.InputCompleted += HandleInputCompleted;
            _keyboardEnabler.InputChanged += HandleInputChanged;

            ReadAllAnnotationsFromDatabase();
        }

        public string AnnotationDirectory { get { return Application.dataPath + "/db/annotations/"; } }

        private void ReadAllAnnotationsFromDatabase()
        {

            var filesInDirectory = Directory.GetFiles(AnnotationDirectory);
            foreach (var file in filesInDirectory)
            {
                if (!file.EndsWith(".json"))
                    continue;

                var newAnnotation = new Annotation(File.ReadAllText(file));
                CreateAnnotationGizmo(newAnnotation);
            }
        }

        private void HandleInputCompleted()
        {
            _keyboardEnabler.Hide();
            _currentAnnotation.Text = _keyboardEnabler._inputField.text;
            _currentAnnotation.ToJson();

            File.WriteAllText(AnnotationDirectory + _currentAnnotation.GUID + ".json", _currentAnnotation.ToJson());
        }


        private void HandleInputChanged(string newText)
        {
            if (_focusedAnnotationGizmo)
                _focusedAnnotationGizmo.UpdateBodyText(newText);
        }


        public void CreateAnnotation(Node contextNode, Vector3 position)
        {
            var newAnnotation = new Annotation()
            {
                Position = position,
                TargetNodeId = contextNode.Id,
                TargetNode = contextNode,
                GUID = System.Guid.NewGuid(),
                ViewPointPosition = new GeoCoordinate()
                {
                    position = Camera.main.transform.position,
                    rotation = Camera.main.transform.eulerAngles,
                },
                AnnotationPosition = new GeoCoordinate() { position = position },
                Author = CurrentUserDefinition._instance != null
                        ? CurrentUserDefinition._instance.CurrentUser
                        : Person.AnonymousUser,
                CreatedAt = DateTime.Now,
            };

            AllAnnotations.Add(newAnnotation);
            _currentAnnotation = newAnnotation;
            _focusedAnnotationGizmo = CreateAnnotationGizmo(newAnnotation);
            _keyboardEnabler.Show();
        }


        private AnnotationGizmo CreateAnnotationGizmo(Annotation annotation)
        {
            var newAnnotationGizmo = Instantiate(_annotationGizmoPrefab);
            newAnnotationGizmo.transform.position = annotation.Position;
            newAnnotationGizmo.transform.SetParent(_gizmoContainer.transform, false);
            newAnnotationGizmo.SetAnnotation(annotation);
            AllAnnotationGizmos.Add(newAnnotationGizmo);
            return newAnnotationGizmo;
        }


        private Annotation _currentAnnotation;
        private AnnotationGizmo _focusedAnnotationGizmo;
        private GameObject _gizmoContainer;
        private KeyboardEnabler _keyboardEnabler;
    }
}
