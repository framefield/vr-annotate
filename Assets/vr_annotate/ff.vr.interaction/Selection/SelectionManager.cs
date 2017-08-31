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
        public event Action<List<ISelectable>> SelectionChangedEvent;
        public event Action<ISelectable> OnHover;
        public event Action<ISelectable> OnUnhover;

        public List<ISelectable> Selection { get { return _selection; } }

        public void SelectItem(ISelectable item)
        {
            foreach (var oldSelected in _selection)
                if (oldSelected != item)
                    oldSelected.IsSelected = false;

            _selection.Clear();

            if (item != null)
            {
                _selection.Add(item);
                item.IsSelected = true;
            }

            if (SelectionChangedEvent != null)
                SelectionChangedEvent(_selection);
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
            _hoveredItem = item;
            OnHover(_hoveredItem);
        }

        public void SetOnUnhover(ISelectable item)
        {
            _hoveredItem = null;
            OnUnhover(_hoveredItem);
        }

        private ISelectable _hoveredItem;
        private List<ISelectable> _selection = new List<ISelectable>();
    }
}
