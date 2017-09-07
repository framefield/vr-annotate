using System;
using System.Collections;
using System.Collections.Generic;
using ff.nodegraph;
using ff.nodegraph.interaction;
using ff.utils;
using ff.vr.annotate.viz;
using UnityEngine;


namespace ff.vr.interaction
{
    /** Handles and broadcasts information about the current selection */
    public class SelectionManager : Singleton<SelectionManager>
    {
        public event Action<Node> OnSelectedNodeChanged;
        public event Action<ISelectable> OnNodeHover;
        public event Action<ISelectable> OnNodeUnhover;

        public event Action<AnnotationGizmo> OnSelectedAnnotationGizmoChanged;
        public event Action<AnnotationGizmo> OnAnnotationGizmoHover;
        public event Action<AnnotationGizmo> OnAnnotationGizmoUnhover;

        public event Action<Vector3> OnNodeSelectionMarkerPositionChanged;

        public Node SelectedNode { get { return _selectedNode; } }
        public AnnotationGizmo SelectedAnnotationGizmo { get { return _selectedAnnotationGizmo; } }


        public void SetSelectedItem(ISelectable item)
        {
            if (item == _selectedNode || item == _selectedAnnotationGizmo)
                return;

            if (item is Node)
            {
                if (_selectedNode != null)
                    _selectedNode.IsSelected = false;

                _selectedAnnotationGizmo = null;
                OnSelectedAnnotationGizmoChanged(null);

                _selectedNode = item as Node;
                _selectedNode.IsSelected = true;
                OnSelectedNodeChanged(_selectedNode);
            }
            else if (item is AnnotationGizmo)
            {
                if (_selectedAnnotationGizmo != null)
                    _selectedAnnotationGizmo.IsSelected = false;

                _selectedAnnotationGizmo = item as AnnotationGizmo;
                _selectedAnnotationGizmo.IsSelected = true;
                // Debug.Log("Selected Gizmo Changed Event");

                OnSelectedAnnotationGizmoChanged(_selectedAnnotationGizmo);
                OnSelectedNodeChanged(_selectedAnnotationGizmo.Annotation.TargetNode);

                SetNodeSelectionMarkerPosition(_selectedAnnotationGizmo.transform.position);
            }
            else if (item == null)
            {
                OnSelectedAnnotationGizmoChanged(null);
            }
        }

        public void SetOnNodeHover(Node node)
        {
            if (node == _hoveredNode)
                return;

            OnNodeHover(node);
            // Debug.Log("OnNode Hover");

            _hoveredNode = node;
        }

        public void SetOnNodeUnhover(Node node)
        {
            if (_hoveredNode != node)
                return;

            OnNodeUnhover(_hoveredNode);
            // Debug.Log("OnNode Un hover");

            _hoveredNode = null;
        }

        public void SetOnAnnotationGizmoHover(AnnotationGizmo annotation)
        {
            if (annotation == _hoveredAnnotationGizmo)
                return;

            OnAnnotationGizmoHover(annotation);
            // Debug.Log("OnAnnotationGizmo Hover");

            _hoveredAnnotationGizmo = annotation;
        }

        public void SetOnAnnotationGizmoUnhover(AnnotationGizmo annotation)
        {
            if (_hoveredAnnotationGizmo != annotation)
                return;

            // Debug.Log("OnAnnotationGizmo Un hover");
            OnAnnotationGizmoUnhover(_hoveredAnnotationGizmo);
            _hoveredAnnotationGizmo = null;
        }

        public void SetNodeSelectionMarkerPosition(Vector3 pos)
        {
            if (pos != _nodeSelectionMarkerPosition)
            {
                _nodeSelectionMarkerPosition = pos;
                OnNodeSelectionMarkerPositionChanged(_nodeSelectionMarkerPosition);
            }
        }

        private Vector3 _nodeSelectionMarkerPosition;

        private Node _hoveredNode;
        private Node _selectedNode;

        private AnnotationGizmo _hoveredAnnotationGizmo;
        private AnnotationGizmo _selectedAnnotationGizmo;

        private Vector3 _selectionMarkerPosition;

        private List<AnnotationPositionRenderer> _allActiveAnnotationPositionRenderer = new List<AnnotationPositionRenderer>();

    }
}
