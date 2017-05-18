using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ff.utils;
using ff.vr.interaction;
using System;

namespace ff.nodegraph.interaction
{
    [RequireComponent(typeof(BoxCollider))]
    public class NodeHitTester : MonoBehaviour, ILaserPointerTarget, IHitTester
    {
        private Node ContextNode = null;

        void Start()
        {
            _annotatableGroups = FindObjectsOfType<NodeGraph>();
            _meshFilter = GetComponent<MeshFilter>();
            _meshRenderer = GetComponent<MeshRenderer>();
            _collider = GetComponent<BoxCollider>();
            _controller = GameObject.FindObjectOfType<SteamVR_TrackedController>();

        }

        private NodeGraph[] _annotatableGroups;
        private BoxCollider _collider;

        void Update()
        {
            if (_controller == null)
            {
                _controller = GameObject.FindObjectOfType<SteamVR_TrackedController>();
            }

            if (_controller == null)
                return;

            //TestHit(_controller.transform);
        }

        /*
        Read carefully! This part is tricky...

        This method is called from LaserPointer on Update() additional to
        a normal Physics.RayCast. If both return hitResults the one with the
        smaller distances will be used. In this case NodeHitTest is called
        with IInteractiveGizmo.PointerEnter(). We then can use the _lastNodeHitByRay.

        To pass as a RayHitresult, NodeHitTester also requires a collider *SIC* 
        To prevent this collider from interfering with other things it should 
        have zero dimension.
         */

        public Node FindAndRememberHit(Ray ray)
        {
            _lastNodeHitByRay = FindHit(ray);
            return _lastNodeHitByRay;
        }


        public Node FindHit(Ray ray)
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
            return FindClosestHit(hits); ;
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

        private void UpdateHighlightForNode(Node node)
        {
            _meshRenderer.enabled = false;
            var bounds = node.CollectGeometryBounds().ToArray();
            _meshFilter.mesh.Clear();
            _meshFilter.mesh = GenerateMeshFromBounds.GenerateMesh(bounds);
            _meshFilter.mesh.RecalculateBounds();
            _meshRenderer.enabled = true;
        }

        public void PointerEnter(LaserPointer pointer)
        {
            UpdateHighlightForNode(_lastNodeHitByRay);
        }

        public void PointerExit(LaserPointer pointer)
        {
            _meshRenderer.enabled = false;
            _lastNodeHitByRay = null;    // really?
        }

        public void PointerUpdate(LaserPointer pointer)
        {
            //var newNode = FindHit(pointer.Ray);
            if (_lastNodeHitByRay != _renderedNode)
            {
                UpdateHighlightForNode(_lastNodeHitByRay);
                _renderedNode = _lastNodeHitByRay;
            }
        }


        private Node _lastNodeHitByRay;
        private Node _renderedNode;
        private MeshFilter _meshFilter;
        private MeshRenderer _meshRenderer;
        private SteamVR_TrackedController _controller;
        private string _lastResult;
        private NodeHitTester _highlighter;
    }
}