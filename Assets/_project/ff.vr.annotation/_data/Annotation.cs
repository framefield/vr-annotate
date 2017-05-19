using System.Collections.Generic;
using ff.nodegraph;
using UnityEngine;

namespace ff.vr.annotate
{
    [System.Serializable]
    public class Annotation
    {
        public System.Guid Id;
        public Vector3 Position;
        public System.Guid ContextNodeId;

        [System.NonSerializedAttribute]
        public Node ContextNode;
    }
}
