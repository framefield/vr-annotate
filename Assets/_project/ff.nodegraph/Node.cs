using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ff.nodegraph
{
    [System.Serializable]
    public class Node
    {
        public Node Parent;

        public Bounds Bounds;
        public bool IsAnnotatable;
        public bool HasGeometry;
        public string Name;
        public bool HasBounds = false;


        public float HitDistance;

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

        // FIXME: this should be the contructor
        public static Node FindChildNodes(GameObject unityObj)
        {
            var node = new Node()
            {
                Name = unityObj.name,
                Children = new Node[unityObj.transform.childCount],
            };
            node.IsAnnotatable = node.CheckIfObjectIsAnnotatable();

            var renderer = unityObj.GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                Debug.Log(renderer.gameObject.name + " > " + renderer.bounds, renderer);
                node.Bounds = renderer.bounds;   // in worldspace
                node.HasBounds = true;
                node.HasGeometry = true;
            }

            for (int index = 0; index < unityObj.transform.childCount; index++)
            {
                var childObj = unityObj.transform.GetChild(index).gameObject;
                var childNode = FindChildNodes(childObj);

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

        public void CollectChildrenIntersectingRay(Ray ray, List<Node> hits)
        {

            if (!this.Bounds.IntersectRay(ray, out HitDistance))
                return;

            // Set back distance to favor selection of smaller object
            if (HitDistance > 0)
            {
                var hitPoint = ray.direction * HitDistance;
                var backed = Vector3.Lerp(hitPoint, Bounds.center, 0.3f);
                var setBackDistance = Vector3.Distance(ray.origin, backed);
                HitDistance = setBackDistance;
            }

            if (this.HasGeometry)
                hits.Add(this);

            foreach (var child in Children)
            {
                child.CollectChildrenIntersectingRay(ray, hits);
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
    }
}