using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ff.vr.annotate;

namespace ff.nodegraph
{
    public class NodeGraph : MonoBehaviour
    {
        public string RootNodeId; // Iri
        public string objStateDescription = "";

        public string modelAuthor = "email@address.com";
        public string modelVersion = "v1";

        [HideInInspector]
        public Node Node;

        private void Start()
        {
        }

        private void Awake()
        {
            CreateNodesFromHierachy();
        }

        private void CreateNodesFromHierachy()
        {
            Node = Node.FindChildNodes(this.gameObject);
        }
    }
}