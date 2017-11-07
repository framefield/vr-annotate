using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ff.vr.annotate;
using System;

namespace ff.nodegraph
{
    public class NodeGraph : MonoBehaviour
    {
        public Guid GUID; // Iri
        public string objStateDescription = "";
        public string modelAuthor = "email@address.com";
        public string modelVersion = "v1";

        [HideInInspector]
        public Node Node;


        private void Awake()
        {
            CreateNodesFromHierachy();
            GUID = new Guid(GUIDHelper.ExtractGUIDFromName(this.name));
        }

        private void CreateNodesFromHierachy()
        {
            Node = Node.FindChildNodes(this.gameObject);
        }

    }
}