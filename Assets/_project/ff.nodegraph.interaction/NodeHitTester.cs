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
    /* A singleton that handles hit-detection with a node-structure.
     */
    public class NodeHitTester : MonoBehaviour, IClickableLaserPointerTarget, IHitTester
    {
        private Node ContextNode = null;
        private Node HoveredNode = null;
        public bool DeepPickingEnabled = false;

        [HideInInspector]
        [NonSerializedAttribute]
        public Dictionary<System.Guid, Node> NodesByGuid = new Dictionary<System.Guid, Node>();

        public TMPro.TextMeshPro Label;

        void Start()
        {
            _annotatableGroups = FindObjectsOfType<NodeGraph>();
            _meshFilter = GetComponent<MeshFilter>();
            _meshRenderer = GetComponent<MeshRenderer>();
            _annotationManager = FindObjectOfType<AnnotationManager>();
        }


        /*
        Read carefully! This part is tricky...

        This method is called from LaserPointer on Update() additional to
        a normal Physics.RayCast. If both return hitResults the one with the
        smaller distance will be used. In this case NodeHitTest is called
        with IInteractiveGizmo.PointerEnter(). We then can use the _lastNodeHitByRay
        to update the visualization respectively.
        */
        public Node FindAndRememberHit(Ray ray)
        {
            _lastNodeHitByRay = FindHit(ray);
            return _lastNodeHitByRay;
        }


        private Node FindHit(Ray ray)
        {
            var hits = new List<Node>();

            if (ContextNode != null)
            {
                ContextNode.CollectChildrenIntersectingRay(ray, hits);
            }
            else
            {
                foreach (var ag in _annotatableGroups)
                {
                    ag.Node.CollectChildrenIntersectingRay(ray, hits);
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

            // Walk up to find child of context
            var n = closestHitNode;
            while (n.Parent != ContextNode && n.Parent != null)
            {
                n = n.Parent;
            }
            n.HitDistance = closestHitNode.HitDistance;
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

        private void UpdateHoverHighlight()
        {
            if (HoveredNode == null)
            {
                Label.gameObject.SetActive(false);
                _meshRenderer.enabled = false;
            }
            else
            {
                Label.text = HoveredNode.Name;
                var bounds = HoveredNode.CollectGeometryBounds().ToArray();
                _meshFilter.mesh = GenerateMeshFromBounds.GenerateMesh(bounds);
                _meshRenderer.enabled = true;
            }
        }

        #region implement LaserInterface
        public void PointerEnter(LaserPointer pointer)
        {
            _trackpadButtonUI = pointer.Controller.gameObject.GetComponentInChildren<TrackpadButtonUI>();
            if (_trackpadButtonUI)
            {
                _trackpadButtonUI.UIButtonClickedEvent += UiButtonClickedHandler;
            }
            Label.gameObject.SetActive(true);
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
            LastHoverPoint = pointer.LastHitPoint;
            Label.transform.position = pointer.LastHitPoint;
            Label.transform.LookAt(Label.transform.position - Camera.main.transform.position + Label.transform.position);
        }


        public void PointerExit(LaserPointer pointer)
        {
            if (_trackpadButtonUI)
            {
                _trackpadButtonUI.UIButtonClickedEvent -= UiButtonClickedHandler;
            }
            HoveredNode = null;
            _meshRenderer.enabled = false;
            _lastNodeHitByRay = null;    // really?
            Label.gameObject.SetActive(false);
        }

        public void PointerTriggered(LaserPointer pointer)
        {
            _annotationManager.CreateAnnotation(HoveredNode, LastHoverPoint);
        }

        public void PointerUntriggered(LaserPointer pointer)
        {

        }
        #endregion

        /** A dummy implementation to simulate entering and exiting hierarchy */
        private void UiButtonClickedHandler(object s, TrackpadButtonUI.ControllerButtons buttonPressed)
        {
            switch (buttonPressed)
            {
                case TrackpadButtonUI.ControllerButtons.Down:
                    this.ContextNode = this.HoveredNode;
                    break;

                case TrackpadButtonUI.ControllerButtons.Up:
                    if (this.ContextNode != null)
                    {
                        this.ContextNode = this.ContextNode.Parent;
                    }
                    break;
            }
        }


        private Vector3 LastHoverPoint;

        private TrackpadButtonUI _trackpadButtonUI;
        private Node _lastNodeHitByRay;
        private Node _renderedNode;
        private MeshFilter _meshFilter;
        private MeshRenderer _meshRenderer;
        private SteamVR_TrackedController _controller;
        private string _lastResult;
        private NodeHitTester _highlighter;
        private NodeGraph[] _annotatableGroups;
        private AnnotationManager _annotationManager;
    }
}