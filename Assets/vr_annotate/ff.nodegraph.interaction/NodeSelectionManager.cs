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
    public class NodeSelectionManager : Singleton<NodeSelectionManager>, IClickableLaserPointerTarget, IHitTester
    {
        public Node HoveredNode = null;
        public bool DeepPickingEnabled = false;
        public Node LastNodeHitByRay;

        public NodeSelectionMarker _selectionMarker;

        [HideInInspector]
        [NonSerializedAttribute]
        public Dictionary<System.Guid, Node> NodesByGuid = new Dictionary<System.Guid, Node>();

        [Header("--- prefab references ----")]

        [SerializeField]
        TMPro.TextMeshPro _hoverLabel;

        [SerializeField]
        Renderer _highlightContextRenderer;

        [SerializeField]
        Renderer _highlightHoverRenderer;


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
            if (SelectionManager.Instance == null)
            {
                throw new UnityException("" + this + " requires a SelectionManager to be initialized. Are you missing an instance of SelectionManager or is the script execution order incorrect?");
            }
            SelectionManager.Instance.SelectionChangedEvent += SelectionChangedHander;
        }


        private void SelectionChangedHander(List<ISelectable> newSelection)
        {
            var nodeOrNull = (newSelection.Count == 1) ? newSelection[0] as Node : null;
            if (nodeOrNull == _selectedNode)
                return;

            _selectedNode = nodeOrNull;

            if (_selectedNode == null)
            {
                _highlightContextRenderer.enabled = false;
            }
            else
            {
                var boundsWithContext = _selectedNode.CollectBoundsWithContext().ToArray();

                _highlightContextRenderer.GetComponent<MeshFilter>().mesh = GenerateMeshFromBounds.GenerateMesh(boundsWithContext);

                _highlightContextRenderer.enabled = true;
            }
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

        public void SetHoveredNode(Node node)
        {
            _hoverLabel.gameObject.SetActive(true);
            HoveredNode = node;
            UpdateHoverHighlight();
        }

        public void SetHoveredNodeToNull()
        {
            _hoverLabel.gameObject.SetActive(false);
            HoveredNode = null;
            UpdateHoverHighlight();
        }


        #region implement LaserInterface
        public void PointerEnter(LaserPointer pointer)
        {
            _hoverLabel.gameObject.SetActive(true);
            HoveredNode = LastNodeHitByRay;
            UpdateHoverHighlight();
        }


        public void PointerUpdate(LaserPointer pointer)
        {
            if (LastNodeHitByRay != _renderedNode)
            {
                HoveredNode = LastNodeHitByRay;
                UpdateHoverHighlight();
                _renderedNode = LastNodeHitByRay;
            }
            _lastHoverPosition = pointer.LastHitPoint;
            _hoverLabel.transform.position = pointer.LastHitPoint;
            _hoverLabel.transform.LookAt(_hoverLabel.transform.position - Camera.main.transform.position + _hoverLabel.transform.position);
        }


        public void PointerExit(LaserPointer pointer)
        {
            HoveredNode = null;
            UpdateHoverHighlight();
            LastNodeHitByRay = null;    // really?
            _hoverLabel.gameObject.SetActive(false);
        }
        #endregion implement LaserInterface


        public void PointerTriggered(LaserPointer pointer)
        {
            if (HoveredNode != null)
            {
                SelectionManager.Instance.SelectItem(HoveredNode);
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
            SelectionManager.Instance.SelectItem(_selectedNode.Parent);
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


        private void UpdateHoverHighlight()
        {
            if (HoveredNode == null)
            {
                _hoverLabel.gameObject.SetActive(false);
                _highlightHoverRenderer.enabled = false;
            }
            else
            {
                _hoverLabel.text = HoveredNode.Name;
                var allBoundsWithContext = HoveredNode.CollectBoundsWithContext().ToArray();

                _highlightHoverRenderer.GetComponent<MeshFilter>().mesh = GenerateMeshFromBounds.GenerateMesh(allBoundsWithContext);

                _highlightHoverRenderer.enabled = true;
            }
        }

        private Vector3 _lastHoverPosition;
        private Node _renderedNode;
        private Node _selectedNode = null;

        [HideInInspector]
        public NodeGraph[] NodeGraphs;
        private AnnotationManager _annotationManager;

    }
}