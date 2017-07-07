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
    public class NodeSelectionManager : MonoBehaviour, IClickableLaserPointerTarget, IHitTester
    {
        private Node SelectedNode = null;
        //private List<Node> SelectedNodes = new List<Node>();
        private Node HoveredNode = null;
        public bool DeepPickingEnabled = false;

        public NodeSelectionMarker _selectionMarker;

        [HideInInspector]
        [NonSerializedAttribute]
        public Dictionary<System.Guid, Node> NodesByGuid = new Dictionary<System.Guid, Node>();

        [Header("--- prefab references ----")]

        [SerializeField]
        TMPro.TextMeshPro _hoverLabel;
        [SerializeField] Renderer _highlightContextRenderer;
        [SerializeField] Renderer _highlightHoverRenderer;

        static NodeSelectionManager _instance = null;

        void Start()
        {
            if (_instance != null)
            {
                throw new UnityException("NodeSelectionManager can only be added once");
            }
            _instance = this;

            _annotatableGroups = FindObjectsOfType<NodeGraph>();
            //_meshFilter = GetComponent<MeshFilter>();
            //_meshRenderer = GetComponent<MeshRenderer>();
            _annotationManager = FindObjectOfType<AnnotationManager>();
        }

        /*
        Read carefully! This part is tricky...

        This method is called from LaserPointer on Update() additional to
        a normal Physics.RayCast. If both return hitResults the one with the
        smaller distance will be used. If this wins, NodeHitTester.PointerEnter(). 
        We then can use the _lastNodeHitByRay to update the visualization respectively.
        */
        public Node FindAndRememberHit(Ray ray)
        {
            _lastNodeHitByRay = FindHit(ray);
            return _lastNodeHitByRay;
        }


        #region implement LaserInterface
        public void PointerEnter(LaserPointer pointer)
        {
            _trackpadButtonUI = pointer.Controller.gameObject.GetComponentInChildren<TrackpadButtonUI>();
            if (_trackpadButtonUI)
            {
                _trackpadButtonUI.UIButtonClickedEvent += UiButtonClickedHandler;
            }
            _hoverLabel.gameObject.SetActive(true);
            HoveredNode = _lastNodeHitByRay;
            UpdateHoverHighlight();
        }

        public void PointerUpdate(LaserPointer pointer)
        {
            if (_lastNodeHitByRay != _renderedNode)
            {
                HoveredNode = _lastNodeHitByRay;
                UpdateHoverHighlight();
                _renderedNode = _lastNodeHitByRay;

            }
            _lastHoverPosition = pointer.LastHitPoint;
            _hoverLabel.transform.position = pointer.LastHitPoint;
            _hoverLabel.transform.LookAt(_hoverLabel.transform.position - Camera.main.transform.position + _hoverLabel.transform.position);
        }


        public void PointerExit(LaserPointer pointer)
        {
            if (_trackpadButtonUI)
            {
                _trackpadButtonUI.UIButtonClickedEvent -= UiButtonClickedHandler;
            }
            HoveredNode = null;
            UpdateHoverHighlight();
            _lastNodeHitByRay = null;    // really?
            _hoverLabel.gameObject.SetActive(false);
        }


        public void PointerTriggered(LaserPointer pointer)
        {
            if (HoveredNode != null)
            {
                //_annotationManager.CreateAnnotation(HoveredNode, LastHoverPoint);
                Debug.Log("Clicked!!!");
                SetSelection(HoveredNode);
                _selectionMarker.SetPosition(_lastHoverPosition);
            }
        }


        public void PointerUntriggered(LaserPointer pointer)
        {

        }
        #endregion implement LaserInterface



        /** A dummy implementation to simulate entering and exiting hierarchy */
        private void UiButtonClickedHandler(object s, TrackpadButtonUI.ControllerButtons buttonPressed)
        {
            switch (buttonPressed)
            {
                case TrackpadButtonUI.ControllerButtons.Down:
                    SetSelection(HoveredNode);
                    break;

                case TrackpadButtonUI.ControllerButtons.Up:
                    SelectParent();
                    break;
            }
        }


        public void SelectParent()
        {
            if (this.SelectedNode != null)
            {
                SetSelection(SelectedNode.Parent);
            }
        }

        #region Hit Detection
        private Node FindHit(Ray ray)
        {
            var hits = new List<Node>();
            if (SelectedNode != null)
            {
                SelectedNode.CollectLeafesIntersectingRay(ray, hits);
            }
            else
            {
                foreach (var ag in _annotatableGroups)
                {
                    ag.Node.CollectLeafesIntersectingRay(ray, hits);
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

            if (closestHitNode == SelectedNode)
            {
                return null;
            }

            // Walk up to find child of context
            var n = closestHitNode;
            while (n.Parent != SelectedNode && n.Parent != null)
            {
                n = n.Parent;
            }
            n.HitDistance = closestHitNode.HitDistance;

            if (n == SelectedNode)
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
                var bounds = HoveredNode.CollectGeometryBounds().ToArray();
                _highlightHoverRenderer.GetComponent<MeshFilter>().mesh = GenerateMeshFromBounds.GenerateMesh(bounds);
                _highlightHoverRenderer.enabled = true;
            }
        }


        private void SetSelection(Node newSelectedNode)
        {
            if (newSelectedNode == SelectedNode)
                return;

            SelectedNode = newSelectedNode;

            if (SelectedNode == null)
            {
                _highlightContextRenderer.enabled = false;
            }
            else
            {
                //_hoverLabel.text = HoveredNode.Name;
                var bounds = SelectedNode.CollectGeometryBounds().ToArray();
                _highlightContextRenderer.GetComponent<MeshFilter>().mesh = GenerateMeshFromBounds.GenerateMesh(bounds);
                _highlightContextRenderer.enabled = true;
            }

            if (_selectionMarker)
            {
                //_selectionMarker.transform.position = _lastHoverPosition;
                _selectionMarker.SetCurrent(newSelectedNode);
            }
        }

        // public void SelectNode(Node newSelection, Vector3 markerPosition)
        // {
        //     _selectionMarker.SetCurrent(newSelection);
        // }

        private Vector3 _lastHoverPosition;

        private TrackpadButtonUI _trackpadButtonUI;
        private Node _lastNodeHitByRay;
        private Node _renderedNode;
        private SteamVR_TrackedController _controller;
        private string _lastResult;
        private NodeSelectionManager _nodeHitTester;
        private NodeGraph[] _annotatableGroups;
        private AnnotationManager _annotationManager;
    }
}