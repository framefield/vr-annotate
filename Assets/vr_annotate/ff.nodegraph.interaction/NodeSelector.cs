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
        public NodeSelectionMarker _selectionMarker;

        [HideInInspector]
        public NodeGraph[] NodeGraphs;

        new void Awake()
        {
            base.Awake();
            NodeGraphs = FindObjectsOfType<NodeGraph>();
            foreach (var ng in NodeGraphs)
            {
                Debug.Log("initializing graph:" + ng.name + " / " + ng.RootNodeId);
            }
            _annotationManager = FindObjectOfType<AnnotationManager>();
        }

        void Start()
        {
            SelectionManager.Instance.SelectedNodeChangedEvent += SelectionChangedHander;
        }


        private void SelectionChangedHander(Node selectedNode)
        {
            if (selectedNode == _selectedNode)
                return;

            _selectedNode = selectedNode;

        }


        public Node FindNodeFromPath(string rootNodeId, string nodePath)
        {
            foreach (var ng in NodeGraphs)
            {
                if (ng.RootNodeId == rootNodeId)
                {
                    var nodeNames = new List<string>(nodePath.Split('/'));
                    var node = ng.Node;

                    // remove rootnode
                    if (node.Name == nodeNames[0])
                    {
                        nodeNames.RemoveAt(0);
                        if (nodeNames.Count == 0)
                            return node;
                    }

                    var stillSearching = true;
                    while (stillSearching)
                    {
                        stillSearching = false;
                        foreach (var child in node.Children)
                        {
                            if (child.Name == nodeNames[0])
                            {
                                nodeNames.RemoveAt(0);
                                if (nodeNames.Count == 0)
                                    return child;

                                node = child;
                                stillSearching = true;
                                break;
                            }
                        }
                    }
                }
            }
            Debug.LogWarningFormat("Scene does not contain reference to: {0} -> {1}", rootNodeId, nodePath);

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

            SelectionManager.Instance.SetOnHover(LastNodeHitByRay);
        }


        public void PointerUpdate(LaserPointer pointer)
        {
            if (LastNodeHitByRay != _renderedNode)
            {
                SelectionManager.Instance.SetOnUnhover(_renderedNode);
                SelectionManager.Instance.SetOnHover(LastNodeHitByRay);


                _renderedNode = LastNodeHitByRay;
            }
            _lastHoverPosition = pointer.LastHitPoint;
        }


        public void PointerExit(LaserPointer pointer)
        {
            SelectionManager.Instance.SetOnUnhover(LastNodeHitByRay);
            LastNodeHitByRay = null;
        }

        #endregion implement LaserInterface


        public void PointerTriggered(LaserPointer pointer)
        {
            if (LastNodeHitByRay != null)
            {
                SelectionManager.Instance.SetSelectedItem(LastNodeHitByRay);
                _selectionMarker.SetPosition(_lastHoverPosition);
            }
        }


        public void PointerUntriggered(LaserPointer pointer)
        {

        }


        public void SelectParentNode()
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
                return null;
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
        private AnnotationManager _annotationManager;

    }
}