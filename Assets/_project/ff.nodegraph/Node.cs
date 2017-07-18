using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using ff.vr.interaction;
using UnityEngine;

namespace ff.nodegraph
{
    [System.Serializable]
    public class Node : ISelectable
    {
        public Bounds Bounds;
        public bool IsAnnotatable;
        public bool HasGeometry;
        public string Name;
        public bool HasBounds = false;

        public System.Guid Id;

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
                Name = unityObj.name,
                Children = new Node[unityObj.transform.childCount],
                Id = new System.Guid(),
                UnityObj = unityObj,
            };
            node.IsAnnotatable = node.CheckIfObjectIsAnnotatable();

            var renderer = unityObj.GetComponent<MeshRenderer>();
            if (renderer != null && unityObj.GetComponent<IgnoreNode>() == null)
            {
                //Debug.Log(renderer.gameObject.name + " > " + renderer.bounds, renderer);
                node.Bounds = renderer.bounds;   // in worldspace
                node.HasBounds = true;
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
                        node.Bounds.Encapsulate(childNode.Bounds);
                        node.HasBounds = true;
                    }
                    else
                    {
                        node.Bounds = childNode.Bounds;
                        node.HasBounds = childNode.HasBounds;
                    }
                }
            }
            return node;
        }

        public void CollectLeavesIntersectingRay(Ray ray, List<Node> hits)
        {

            if (!this.Bounds.IntersectRay(ray, out HitDistance))
                return;

            // Set back distance to favor selection of smaller object
            // if (HitDistance > 0)
            // {
            //     var hitPoint = ray.origin + ray.direction * HitDistance;
            //     var backed = Vector3.Lerp(hitPoint, Bounds.center, 0.98f);
            //     var backDistance = Vector3.Distance(ray.origin, backed);
            //     HitDistance = backDistance;
            // }

            if (this.HasGeometry)
                hits.Add(this);

            if (Children == null)
                return;

            foreach (var child in Children)
            {
                child.CollectLeavesIntersectingRay(ray, hits);
            }
        }


        public bool CheckIfObjectIsAnnotatable()
        {
            //GameObject obj = this.UnityObj;
            return true;
        }

        public List<Bounds> CollectGeometryBounds(List<Bounds> result = null)
        {
            if (result == null)
                result = new List<Bounds>();

            if (this.HasGeometry)
            {
                result.Add(this.Bounds);
            }

            foreach (var c in Children)
            {
                c.CollectGeometryBounds(result);
            }
            return result;
        }


        public Vector3 GetPosition()
        {
            return this.Bounds.center;
        }
    }
}