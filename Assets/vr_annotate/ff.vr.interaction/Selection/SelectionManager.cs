using System.Collections;
using System.Collections.Generic;
using ff.utils;
using UnityEngine;

namespace ff.vr.interaction
{
    /** Handles and broadcasts information about the current selection */
    public class SelectionManager : Singleton<SelectionManager>
    {
        public delegate void SelectionChanged(List<ISelectable> newSelection);
        public SelectionChanged SelectionChangedEvent;


        public List<ISelectable> Selection { get { return _selection; } }

        public void SelectItem(ISelectable item)
        {
            // Clear old selection (e.g. send property setter updates)
            foreach (var oldSelected in _selection)
            {
                if (oldSelected != item)
                {
                    oldSelected.IsSelected = false;
                }
            }

            _selection.Clear();

            if (item != null)
            {
                _selection.Add(item);
                item.IsSelected = true;
            }

            if (SelectionChangedEvent != null)
                SelectionChangedEvent(_selection);
        }

        private List<ISelectable> _selection = new List<ISelectable>();
    }
}
