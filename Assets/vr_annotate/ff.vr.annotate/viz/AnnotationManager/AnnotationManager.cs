using System.Collections.Generic;
using UnityEngine;
using ff.vr.annotate;
using ff.nodegraph;
using ff.vr.interaction;
using System;
using System.IO;
using ff.location;
using ff.utils;
using ff.vr.annotate.datamodel;
using ff.vr.annotate.helpers;
using ff.nodegraph.interaction;

namespace ff.vr.annotate.viz
{
    /* 
        A singleton, that handles loading and adding annotations
        - Can be selected.
        - Can be hovered.
        - We be generated on the fly
        - Is grouped under a Annotations-Group
    */
    public class AnnotationManager : Singleton<AnnotationManager>
    {
        public string SimulatedYear = "300 BC";
        public string SimulatedTimeOfDay = "18:23:12";

        private List<Annotation> AllAnnotations = new List<Annotation>();
        private List<AnnotationGizmo> AllAnnotationGizmos = new List<AnnotationGizmo>();

        public AnnotationGizmo _annotationGizmoPrefab;
        public bool OnStartupReadAllAnnotationFromDataBase;


        void Start()
        {
            _gizmoContainer = new GameObject();
            _gizmoContainer.name = "GizmoContainer";
            _gizmoContainer.transform.SetParent(this.transform, false);
            _keyboardEnabler = FindObjectOfType<KeyboardEnabler>();
            _keyboardEnabler.Hide();
            _keyboardEnabler.InputCompleted += HandleInputCompleted;
            _keyboardEnabler.InputChanged += HandleInputChanged;

            if (OnStartupReadAllAnnotationFromDataBase)
                ReadAllAnnotationsFromDatabase();
        }

        public string AnnotationDirectory { get { return Application.dataPath + "/db/annotations/"; } }


        private void ReadAllAnnotationsFromDatabase()
        {
            var filesInDirectory = Directory.GetFiles(AnnotationDirectory, "*.json");
            foreach (var file in filesInDirectory)
            {
                var newAnnotation = new Annotation(File.ReadAllText(file));
                if (newAnnotation.TargetNode == null)
                    continue;

                CreateAnnotationGizmo(newAnnotation);
            }
        }


        private void HandleInputCompleted()
        {
            _keyboardEnabler.Hide();
            _lastCreatedAnnotation.Text = _keyboardEnabler._inputField.text;
            _lastCreatedAnnotation.ToJson();

            File.WriteAllText(AnnotationDirectory + _lastCreatedAnnotation.Guid + ".json", _lastCreatedAnnotation.ToJson());
        }


        private void HandleInputChanged(string newText)
        {
            if (_focusedAnnotationGizmo)
                _focusedAnnotationGizmo.UpdateBodyText(newText);
        }

        public void CreateDummyAnnotation(Node contextNode, Vector3 position)
        {
            var newAnnotation = new Annotation()
            {
                TargetNodeId = contextNode.GUID,
                TargetNode = contextNode,
                Guid = System.Guid.NewGuid(),

                AnnotationPosition = new GeoCoordinate() { position = position, positionViewport = Camera.main.transform.position },

                Author = CurrentUserDefinition._instance != null
                                  ? CurrentUserDefinition._instance.CurrentUser
                                  : Person.AnonymousUser,
                CreatedAt = DateTime.Now,
            };

            AllAnnotations.Add(newAnnotation);
            _lastCreatedAnnotation = newAnnotation;
            _focusedAnnotationGizmo = CreateAnnotationGizmo(newAnnotation);
        }


        public void CreateAnnotation(Node contextNode, Vector3 position)
        {
            var newAnnotation = new Annotation()
            {
                TargetNodeId = contextNode.GUID,
                TargetNode = contextNode,
                Guid = System.Guid.NewGuid(),

                AnnotationPosition = new GeoCoordinate() { position = position, positionViewport = Camera.main.transform.position },

                Author = CurrentUserDefinition._instance != null
                        ? CurrentUserDefinition._instance.CurrentUser
                        : Person.AnonymousUser,
                CreatedAt = DateTime.Now,
            };

            AllAnnotations.Add(newAnnotation);
            _lastCreatedAnnotation = newAnnotation;
            _focusedAnnotationGizmo = CreateAnnotationGizmo(newAnnotation);
            _keyboardEnabler.Show();
        }


        private AnnotationGizmo CreateAnnotationGizmo(Annotation annotation)
        {
            var newAnnotationGizmo = Instantiate(_annotationGizmoPrefab);
            newAnnotationGizmo.transform.position = annotation.AnnotationPosition.position;
            newAnnotationGizmo.transform.SetParent(_gizmoContainer.transform, false);
            newAnnotationGizmo.Annotation = annotation;
            AllAnnotationGizmos.Add(newAnnotationGizmo);
            return newAnnotationGizmo;
        }


        public void GoToNextAnnotation(Teleportation teleportation)
        {
            var selectedAnnotationGizmo = SelectionManager.Instance.SelectedAnnotationGizmo;
            if (selectedAnnotationGizmo == null)
                return;

            var nextGizmo = GetNextAnnotationGizmoOnNode(selectedAnnotationGizmo);
            SelectionManager.Instance.SetSelectedItem(nextGizmo);
            teleportation.JumpToPosition(nextGizmo.Annotation.AnnotationPosition.positionViewport);
        }

        public AnnotationGizmo GetNextAnnotationGizmoOnNode(AnnotationGizmo gizmo)
        {
            var relevantAnnotations = GetAllAnnotationsGizmosOnNode(gizmo.Annotation.TargetNode);
            int i = 0;
            while (relevantAnnotations[i] != gizmo)
                i++;
            return relevantAnnotations[(i + 1) % relevantAnnotations.Count];
        }

        public void GoToPreviousAnnotation(Teleportation teleportation)
        {
            var selectedAnnotationGizmo = SelectionManager.Instance.SelectedAnnotationGizmo;
            if (selectedAnnotationGizmo == null)
                return;

            var nextGizmo = GetPreviousAnnotationGizmoOnNode(selectedAnnotationGizmo);
            SelectionManager.Instance.SetSelectedItem(nextGizmo);
            teleportation.JumpToPosition(nextGizmo.Annotation.AnnotationPosition.positionViewport);
        }

        public AnnotationGizmo GetPreviousAnnotationGizmoOnNode(AnnotationGizmo gizmo)
        {
            var relevantAnnotations = GetAllAnnotationsGizmosOnNode(gizmo.Annotation.TargetNode);
            int i = 0;
            while (relevantAnnotations[i] != gizmo)
                i++;
            return relevantAnnotations[(i - 1 + relevantAnnotations.Count) % relevantAnnotations.Count];
        }


        public List<AnnotationGizmo> GetAllAnnotationsGizmosOnNode(Node node)
        {
            List<AnnotationGizmo> annotationsThatTargetNode = new List<AnnotationGizmo>();
            foreach (var a in GetComponentsInChildren<AnnotationGizmo>())
            {
                if (a.Annotation.TargetNode == node)
                    annotationsThatTargetNode.Add(a);
            }
            return annotationsThatTargetNode;
        }


        private Annotation _lastCreatedAnnotation;
        private AnnotationGizmo _focusedAnnotationGizmo;
        private GameObject _gizmoContainer;
        private KeyboardEnabler _keyboardEnabler;
    }
}
