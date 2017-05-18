using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ff.utils;

namespace ff.nodegraph
{
    public class NodeHitTester : MonoBehaviour
    {
        void Start()
        {
            _annotatableGroups = FindObjectsOfType<NodeGraph>();
            _meshFilter = this.GetComponent<MeshFilter>();
            _meshRenderer = this.GetComponent<MeshRenderer>();
            _controller = GameObject.FindObjectOfType<SteamVR_TrackedController>();
        }

        private NodeGraph[] _annotatableGroups;

        void Update()
        {
            if (_controller == null)
            {
                _controller = GameObject.FindObjectOfType<SteamVR_TrackedController>();
            }

            if (_controller == null)
                return;

            TestHit(_controller.transform);
        }

        private void TestHit(Transform t)
        {
            var ray = new Ray(t.position, t.forward);
            var hits = new List<Node>();

            foreach (var ag in _annotatableGroups)
            {
                ag.Node.CollectChildrenIntersectingRay(ray, hits);
            }


            if (PrintHitsIfChanged(hits))
            {
                var closestHit = FindClosestHit(hits);
                var isHittingSomething = (closestHit != null);
                if (isHittingSomething)
                {
                    var bounds = closestHit.CollectGeometryBounds().ToArray();
                    _meshFilter.mesh = GenerateMeshFromBounds.GenerateMesh(bounds);
                }
                _meshRenderer.enabled = isHittingSomething;
            }
        }

        private Node FindClosestHit(List<Node> hits)
        {
            hits.Sort((h1, h2) => (h1.HitDistance).CompareTo(h2.HitDistance));

            foreach (var h in hits)
            {
                if (h.HitDistance > 0)
                    return h;
            }
            return null;
        }


        private bool PrintHitsIfChanged(List<Node> hits)
        {
            var sb = new System.Text.StringBuilder();
            var separator = "";
            foreach (var h in hits)
            {
                sb.Append(separator);
                sb.Append(h.Name + " (" + h.HitDistance + ")  ");
                separator = ", ";
            }
            var r = sb.ToString();

            bool resultChanged = r != _lastResult;
            if (resultChanged)
            {
                _lastResult = r;
                //Debug.Log(r);
                return true;
            }
            return resultChanged;
        }

        private MeshFilter _meshFilter;
        private MeshRenderer _meshRenderer;
        private SteamVR_TrackedController _controller;
        private string _lastResult;
        private NodeHitTester _highlighter;
    }
}