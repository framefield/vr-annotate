using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using ff.vr.annotate.viz;
using ff.vr.interaction;
using UnityEngine;

namespace ff.nodegraph
{
    [System.Serializable]
    public class Node : ISelectable
    {
        public struct BoundsWithContextStruct
        {
            public Bounds Bounds;
            public Bounds LocalBounds;
            public Transform LocalTransform;
            public bool HasLocalBounds;
            public MeshCollider MeshCollider;
            public bool HasMeshCollider;
        }

        public BoundsWithContextStruct BoundsWithContext = new BoundsWithContextStruct();
        public bool IsAnnotatable;
        public bool HasGeometry;
        public string Name;
        public bool HasBounds = false;
        public System.Guid GUID;
        public float HitDistance;

        [System.NonSerializedAttribute]
        public Node Parent;

        [System.NonSerializedAttribute]
        public Node[] Children;

        [System.NonSerializedAttribute]
        public GameObject UnityObj;

        public void PrintStructure(int level = 0)
        {
            Debug.LogFormat(new System.String(' ', level) + this.Name);
            foreach (var c in Children)
            {
                c.PrintStructure(level + 1);
            }
        }

        public Node RootNode
        {
            get
            {
                var root = this;
                while (root.Parent != null)
                {
                    root = root.Parent;
                }
                return root;
            }
        }

        public bool IsSelected { get; set; }

        public NodeGraph NodeGraphRoot
        {
            get
            {
                if (RootNode == null || RootNode.UnityObj == null)
                    return null;

                return RootNode.UnityObj.GetComponent<NodeGraph>();
            }
        }

        public string NodePath
        {
            get
            {
                var sb = new StringBuilder();

                var n = this;
                sb.Append(n.Name);
                while (n.Parent != null)
                {
                    n = n.Parent;
                    sb.Insert(0, "/");
                    sb.Insert(0, n.Name);
                }

                return sb.ToString();
            }
        }

        // FIXME: this should be the contructor
        public static Node FindChildNodes(GameObject unityObj)
        {
            var node = new Node()
            {
                Children = new Node[unityObj.transform.childCount],
                UnityObj = unityObj,
                GUID = new Guid(GUIDGenerator.ExtractGUIDFromName(unityObj.name)),
                Name = GUIDGenerator.RemoveGUIDFromName(unityObj.name)
            };

            // unityObj.name = node.Name; // write name without GUID back to Object
            node.IsAnnotatable = node.CheckIfObjectIsAnnotatable();

            var renderer = unityObj.GetComponent<MeshRenderer>();
            if (renderer != null && unityObj.GetComponent<IgnoreNode>() == null)
            {
                node.HasBounds = true;

                node.BoundsWithContext.Bounds = renderer.bounds;

                var meshfilter = unityObj.GetComponent<MeshFilter>();

                if (node.BoundsWithContext.HasLocalBounds = meshfilter != null)
                {
                    node.BoundsWithContext.LocalBounds = meshfilter.mesh.bounds;
                    node.BoundsWithContext.LocalTransform = unityObj.transform;
                }

                var meshCollider = unityObj.GetComponent<MeshCollider>();
                if (meshCollider == null && meshfilter != null)
                {
                    meshCollider = unityObj.AddComponent<MeshCollider>();
                }

                if (node.BoundsWithContext.HasMeshCollider = meshCollider != null)
                {
                    node.BoundsWithContext.MeshCollider = meshCollider;
                }

                node.HasGeometry = true;
            }

            for (int index = 0; index < unityObj.transform.childCount; index++)
            {
                var childObj = unityObj.transform.GetChild(index).gameObject;
                var childNode = FindChildNodes(childObj);
                childNode.Parent = node;

                node.Children[index] = childNode;

                if (childNode.HasBounds)
                {
                    if (node.HasBounds)
                    {
                        node.BoundsWithContext.Bounds.Encapsulate(childNode.BoundsWithContext.Bounds);
                        node.HasBounds = true;
                    }
                    else
                    {
                        node.BoundsWithContext.Bounds = childNode.BoundsWithContext.Bounds;
                        node.HasBounds = childNode.HasBounds;
                    }
                }
            }
            return node;
        }

        public void CollectLeavesIntersectingRay(Ray ray, List<Node> hits)
        {
            if (!this.BoundsWithContext.Bounds.IntersectRay(ray, out HitDistance))
                return;

            if (this.BoundsWithContext.HasLocalBounds)
            {
                var localRayOrigin = UnityObj.transform.InverseTransformPoint(ray.origin);
                var localRayDirection = UnityObj.transform.InverseTransformDirection(ray.direction);
                if (!this.BoundsWithContext.LocalBounds.IntersectRay(new Ray(localRayOrigin, localRayDirection), out HitDistance))
                    return;
            }

            if (this.BoundsWithContext.HasMeshCollider)
            {
                RaycastHit hit;
                var hasHit = this.BoundsWithContext.MeshCollider.Raycast(ray, out hit, 100f);
                HitDistance = hit.distance;
                if (!hasHit)
                    return;

            }

            if (this.HasGeometry)
            {
                hits.Add(this);

            }

            if (Children == null)
                return;

            foreach (var child in Children)
            {
                child.CollectLeavesIntersectingRay(ray, hits);
            }
        }


        public bool CheckIfObjectIsAnnotatable()
        {
            // todo!!
            return true;
        }

        public List<BoundsWithContextStruct> CollectBoundsWithContext(List<BoundsWithContextStruct> result = null)
        {
            if (result == null)
                result = new List<BoundsWithContextStruct>();

            if (this.HasGeometry)
            {
                result.Add(this.BoundsWithContext);
            }

            foreach (var c in Children)
            {
                c.CollectBoundsWithContext(result);
            }
            return result;
        }

        public Vector3 GetPosition()
        {
            return this.BoundsWithContext.Bounds.center;
        }

    }
}