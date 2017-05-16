using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ff.vr.annotation
{
    [RequireComponent(typeof(AnnotatableGroup))]
    public class GroupHitTester : MonoBehaviour
    {
        void Start()
        {
            _annotatableGroup = this.GetComponent<AnnotatableGroup>();
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

            var sb = new System.Text.StringBuilder();

            var separator = "";
            foreach (var h in hits)
            {
                sb.Append(separator);
                sb.Append(h.Name);
                separator = ", ";
            }
            var r = sb.ToString();
            if (r != _lastResult)
            {
                _lastResult = r;
                Debug.Log(r);
            }
        }


        private AnnotatableGroup _annotatableGroup;
        private SteamVR_TrackedController _controller;
        private string _lastResult;
    }
}
