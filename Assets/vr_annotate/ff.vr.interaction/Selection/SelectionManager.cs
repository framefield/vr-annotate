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
        public event Action<AnnotationGizmo> SelectedAnnotationGizmoChangedEvent;
        public event Action<ISelectable> OnHover;
        public event Action<ISelectable> OnUnhover;

        public List<ISelectable> Selection { get { return _selection; } }

        public Node SelectedNode;
        public AnnotationGizmo SelectedAnnotationGizmo;

        public void SelectItem(ISelectable item)
        {
            if (item == SelectedNode || item == SelectedAnnotationGizmo)
                return;

            if (item is Node)
            {
                SelectedNode.IsSelected = false;
                SelectedNode = item as Node;
                SelectedNode.IsSelected = true;
                SelectedNodeChangedEvent(SelectedNode);
            }

            else if (item is AnnotationGizmo)
            {
                SelectedAnnotationGizmo.IsSelected = false;
                SelectedAnnotationGizmo = item as AnnotationGizmo;
                SelectedAnnotationGizmo.IsSelected = true;
                SelectedAnnotationGizmoChangedEvent(SelectedAnnotationGizmo);
            }
        }

        public Node GetSelectedNode()
        {
            if (Selection.Count == 0)
                return null;

            var selectedItem = Selection[0];
            if (selectedItem is Node)
                return selectedItem as Node;

            if (selectedItem is AnnotationGizmo)
            {
                var annotation = (AnnotationGizmo)selectedItem;
                return annotation.Annotation.TargetNode;
            }

            return null;
        }

        public void SetOnHover(ISelectable item)
        {
            if (item == _hoveredItem)
                return;

            if (item is Node)
                Debug.Log("setting on hover " + (item as Node).Name);
            // OnUnhover(_hoveredItem);
            OnHover(item);
            _hoveredItem = item;
        }

        public void SetOnUnhover(ISelectable item)
        {
            if (_hoveredItem != item)
                return;

            if (item is Node)
                Debug.Log("setting on unHover " + (item as Node).Name);
            OnUnhover(_hoveredItem);
            _hoveredItem = null;
        }

        private ISelectable _hoveredItem;
        private List<ISelectable> _selection = new List<ISelectable>();
    }
}
