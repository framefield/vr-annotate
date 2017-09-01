using System;
using System.Collections;
using System.Collections.Generic;
using ff.nodegraph;
using ff.nodegraph.interaction;
using ff.utils;
using ff.vr.annotate;
using UnityEngine;


namespace ff.vr.interaction
{
    /** Handles and broadcasts information about the current selection */
    public class SelectionManager : Singleton<SelectionManager>
    {
        public event Action<Node> SelectedNodeChangedEvent;
        public event Action<ISelectable> OnNodeHover;
        public event Action<ISelectable> OnNodeUnhover;

        public event Action<AnnotationGizmo> SelectedAnnotationGizmoChangedEvent;
        public event Action<AnnotationGizmo> OnAnnotationGizmoHover;
        public event Action<AnnotationGizmo> OnAnnotationGizmoUnhover;

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

                _selectedNode = item as Node;
                _selectedNode.IsSelected = true;
                SelectedNodeChangedEvent(_selectedNode);
            }
            else if (item is AnnotationGizmo)
            {
                if (_selectedAnnotationGizmo != null)
                    _selectedAnnotationGizmo.IsSelected = false;

                _selectedAnnotationGizmo = item as AnnotationGizmo;
                _selectedAnnotationGizmo.IsSelected = true;
                SelectedAnnotationGizmoChangedEvent(_selectedAnnotationGizmo);
            }
        }

        public void SetOnNodeHover(Node node)
        {
            if (node == _hoveredNode)
                return;

            OnNodeHover(node);
            _hoveredNode = node;
        }

        public void SetOnNodeUnhover(Node node)
        {
            if (_hoveredNode != node)
                return;

            OnNodeUnhover(_hoveredNode);
            _hoveredNode = null;
        }

        public void SetOnAnnotationGizmoHover(AnnotationGizmo annotation)
        {
            if (annotation == _hoveredAnnotationGizmo)
                return;

            OnAnnotationGizmoHover(annotation);
            SelectedAnnotationGizmoChangedEvent(null);
            _hoveredAnnotationGizmo = annotation;
        }

        public void SetOnAnnotationGizmoUnhover(AnnotationGizmo annotation)
        {
            if (_hoveredAnnotationGizmo != annotation)
                return;

            OnAnnotationGizmoUnhover(_hoveredAnnotationGizmo);
            _hoveredAnnotationGizmo = null;
        }

        private Node _hoveredNode;
        private Node _selectedNode;

        private AnnotationGizmo _hoveredAnnotationGizmo;
        private AnnotationGizmo _selectedAnnotationGizmo;

        private Vector3 _selectionMarkerPosition;

        public List<AnnotationPositionRenderer> _allActiveAnnotationPositionRenderer = new List<AnnotationPositionRenderer>();

    }
}
