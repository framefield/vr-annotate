using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using ff.utils;
using ff.vr.interaction;
using ff.vr.annotate.viz;

using ff.vr.annotate;

namespace ff.nodegraph.interaction
{
    /** A singleton that handles hit-detection within a node-structure.  */
    public class NodeSelector : Singleton<NodeSelector>, IClickableLaserPointerTarget, IHitTester
    {
        public bool DeepPickingEnabled = false;
        public Node LastNodeHitByRay;

        [HideInInspector]
        public NodeGraph[] NodeGraphs;

        new void Awake()
        {
            base.Awake();
            NodeGraphs = FindObjectsOfType<NodeGraph>();
        }

        void Start()
        {
            SelectionManager.Instance.OnSelectedNodeChanged += SelectionChangedHander;
        }


        private void SelectionChangedHander(Node selectedNode)
        {
            if (selectedNode == _selectedNode)
                return;

            _selectedNode = selectedNode;

        }


        public Node FindNodeFromGuidPath(string path)
        {
            var guids = new List<string>(path.Split('/'));
            foreach (var ng in NodeGraphs)
            {
                if (ng.GUID.ToString() == guids[0])
                {
                    var guidsCopy = guids;
                    var node = ng.Node;
                    guidsCopy.RemoveAt(0);

                    while (guidsCopy.Count > 1)
                    {
                        var foundNextChild = false;
                        foreach (var child in node.Children)
                        {
                            if (child.GUID.ToString() == guidsCopy[0])
                            {
                                guidsCopy.RemoveAt(0);
                                node = child;
                                foundNextChild = true;
                                if (guidsCopy.Count <= 1)
                                {
                                    return node;
                                }
                            }
                        }
                        if (!foundNextChild)
                        {
                            Debug.LogErrorFormat("Scene does not contain reference to: {0} -> {1}", node.GUID, path);
                            return null;
                        }
                    }
                    return node;
                }
            }
            return null;
        }



        /*
        Read carefully! This part is tricky...

        This method is called from LaserPointer on Update() additional to
        a normal Physics.RayCast. If both return hitResults the one with the
        smaller distance will be used. If NodeSelectionManager wins .PointerEnter() is called. 
        We then can use the _lastNodeHitByRay to update the visualization respectively.
        */

        #region implement LaserInterface
        public void PointerEnter(LaserPointer pointer)
        {

            SelectionManager.Instance.SetOnNodeHover(LastNodeHitByRay);
        }


        public void PointerUpdate(LaserPointer pointer)
        {
            if (LastNodeHitByRay != _renderedNode)
            {
                SelectionManager.Instance.SetOnNodeUnhover(_renderedNode);
                SelectionManager.Instance.SetOnNodeHover(LastNodeHitByRay);


                _renderedNode = LastNodeHitByRay;
            }
            _lastHoverPosition = pointer.LastHitPoint;
        }


        public void PointerExit(LaserPointer pointer)
        {
            SelectionManager.Instance.SetOnNodeUnhover(LastNodeHitByRay);
            LastNodeHitByRay = null;
        }

        #endregion implement LaserInterface


        public void PointerTriggered(LaserPointer pointer)
        {
            if (LastNodeHitByRay != null)
            {
                var hitParentOfSelected = SelectionManager.Instance.SelectedNode != null && LastNodeHitByRay == SelectionManager.Instance.SelectedNode.Parent;
                if (!hitParentOfSelected)
                    SelectionManager.Instance.SetNodeSelectionMarkerPosition(_lastHoverPosition);

                SelectionManager.Instance.SetSelectedItem(LastNodeHitByRay);
            }
        }


        public void PointerUntriggered(LaserPointer pointer)
        {

        }


        public void HandleNullHit()
        {
            if (this._selectedNode == null)
            {
                Debug.LogWarning("Tried to select parent when no selected?");
                return;
            }

            if (this._selectedNode.Parent == null)
            {
                Debug.LogWarning("Tried to select parent when current selection had no parent?");
                return;
            }
            SelectionManager.Instance.SetSelectedItem(_selectedNode.Parent);
            SelectionManager.Instance.SetSelectedItem(null);

        }


        #region Hit Detection
        public Node FindHit(Ray ray)
        {
            var hits = new List<Node>();
            if (_selectedNode != null)
            {
                _selectedNode.CollectLeavesIntersectingRay(ray, hits);
            }
            else
            {
                foreach (var ng in NodeGraphs)
                {
                    ng.Node.CollectLeavesIntersectingRay(ray, hits);
                }
            }

            if (hits.Count == 0)
                return null;

            var closestHitNode = FindClosestHit(hits);

            if (closestHitNode == null)
            {
                return null;
            }

            if (DeepPickingEnabled)
            {
                return closestHitNode;
            }

            if (closestHitNode == _selectedNode)
            {
                return closestHitNode;
            }

            // Walk up to find child of context
            var n = closestHitNode;
            while (n.Parent != _selectedNode && n.Parent != null)
            {
                n = n.Parent;
            }
            n.HitDistance = closestHitNode.HitDistance;

            if (n == _selectedNode)
            {
                Debug.Log("self selection!");
            }
            return n;
        }


        private Node FindClosestHit(List<Node> hits)
        {
            hits.Sort((h1, h2) => (h1.HitDistance).CompareTo(h2.HitDistance));

            foreach (var h in hits)
            {
                if (h.HitDistance > 0)
                {
                    return h;
                }
            }
            return null;
        }
        #endregion


        private Vector3 _lastHoverPosition;
        private Node _renderedNode;
        private Node _selectedNode = null;

    }
}