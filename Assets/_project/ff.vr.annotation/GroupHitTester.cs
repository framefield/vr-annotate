using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ff.utils;

namespace ff.vr.annotation
{
    [RequireComponent(typeof(AnnotatableGroup))]
    public class GroupHitTester : MonoBehaviour
    {
        void Start()
        {
            _annotatableGroup = this.GetComponent<AnnotatableGroup>();
            _meshFilter = this.GetComponent<MeshFilter>();
            _controller = GameObject.FindObjectOfType<SteamVR_TrackedController>();
        }


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
            var hits = new List<AnnotatableNode>();
            _annotatableGroup.Node.CollectChildrenIntersectingRay(ray, hits);

            if (PrintHitsIfChanged(hits))
            {
                if (_meshFilter && hits.Count > 0)
                {
                    var node = hits[0];

                    var bounds = node.CollectGeometryBounds().ToArray();
                    _meshFilter.mesh = GenerateMeshFromBounds.GenerateMesh(bounds);
                }
            }
        }

        private MeshFilter _meshFilter;

        private bool PrintHitsIfChanged(List<AnnotatableNode> hits)
        {
            var sb = new System.Text.StringBuilder();
            var separator = "";
            foreach (var h in hits)
            {
                sb.Append(separator);
                sb.Append(h.Name);
                separator = ", ";
            }
            var r = sb.ToString();

            bool resultChanged = r != _lastResult;
            if (resultChanged)
            {
                _lastResult = r;
                Debug.Log(r);
                return true;
            }
            return resultChanged;
        }




        private AnnotatableGroup _annotatableGroup;
        private SteamVR_TrackedController _controller;
        private string _lastResult;
    }
}
