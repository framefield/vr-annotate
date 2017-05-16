using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ff.vr.annotation
{
    [System.Serializable]
    public class AnnotatableNode
    {
        public AnnotatableNode Parent;
        public AnnotatableNode[] Children;
        public Bounds Bounds;
        public bool IsAnnotatable;
        public string Name;
        public bool HasBounds = false;

        [System.NonSerializedAttribute]
        public GameObject ObjectReference;

        public void PrintStructure(int level = 0)
        {
            Debug.LogFormat(new System.String(' ', level) + this.Name);
            foreach (var c in Children)
            {
                c.PrintStructure(level + 1);
            }
        }
    }
}