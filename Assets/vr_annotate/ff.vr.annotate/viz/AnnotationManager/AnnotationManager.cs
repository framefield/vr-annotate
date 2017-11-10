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
using System.Collections;
using UnityEngine.Networking;

namespace ff.vr.annotate.viz
{
    /** 
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

        private List<AnnotationGizmo> AllAnnotationGizmos = new List<AnnotationGizmo>();

        public AnnotationGizmo _annotationGizmoPrefab;

        void Start()
        {
            InitGizmoContainer();
            _keyboardEnabler = FindObjectOfType<KeyboardEnabler>();
            _keyboardEnabler.Hide();
            _keyboardEnabler.InputCompleted += HandleInputCompleted;
            _keyboardEnabler.InputChanged += HandleInputChanged;
        }

        private void InitGizmoContainer()
        {
            _gizmoContainer = new GameObject();
            _gizmoContainer.name = "GizmoContainer";
            _gizmoContainer.transform.SetParent(this.transform, false);
        }

        private void HandleInputCompleted()
        {
            _keyboardEnabler.Hide();
            _lastCreatedAnnotation.Text = _keyboardEnabler._inputField.text;
            var databaseLocation = _lastCreatedAnnotation.TargetNode.UnityObj.GetComponentInParent<Target>().DataBaseLocationToUse;

            switch (databaseLocation)
            {
                case Target.DataBaseLocation.localDirectory:
                    Serialization.WriteAnnotationToLocalDirectory(_lastCreatedAnnotation);
                    break;
                case Target.DataBaseLocation.REST:
                    StartCoroutine(Serialization.WriteAnnotationToServer(_lastCreatedAnnotation));
                    break;
            }
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
                TargetNode = contextNode,
                JsonLdId = new LinkedDataID(LinkedDataID.IDType.Annotation),

                AnnotationPosition = new GeoCoordinate() { position = position },
                ViewPortPosition = new GeoCoordinate() { position = Camera.main.transform.position },

                Author = CurrentUserDefinition._instance != null
                        ? CurrentUserDefinition._instance.CurrentUser
                        : Person.AnonymousUser,
                CreatedAt = DateTime.Now,
            };
            Debug.Log("Created new annotation: " + newAnnotation.JsonLdId);

            _lastCreatedAnnotation = newAnnotation;
            _focusedAnnotationGizmo = CreateAnnotationGizmo(newAnnotation);
            _keyboardEnabler.Show();
        }


        public AnnotationGizmo CreateAnnotationGizmo(Annotation annotation)
        {
            if (_gizmoContainer == null)
                InitGizmoContainer();

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
            teleportation.JumpToPosition(nextGizmo.Annotation.ViewPortPosition.position);
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
            teleportation.JumpToPosition(nextGizmo.Annotation.ViewPortPosition.position);
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
