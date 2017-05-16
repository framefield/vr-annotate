
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ff.vr.annotation;

namespace ff.vr.annotation
{
    public class AnnotatableGroup : MonoBehaviour
    {
        public string DB_ReferenceID;

        [HideInInspector]
        public AnnotatableNode Node;

        private void Start()
        {
        }

        private void Awake()
        {
            CreateNodesFromHierachy();
        }

        private void CreateNodesFromHierachy()
        {
            Node = AnnotatableNode.FindChildNodes(this.gameObject);
            //Node.PrintStructure();
        }
    }
}