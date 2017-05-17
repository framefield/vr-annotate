using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ff.vr.annotation
{
    [System.Serializable]
    public class AnnotatableNode
    {
        public AnnotatableNode Parent;

        public Bounds Bounds;
        public bool IsAnnotatable;
        public bool HasGeometry;
        public string Name;
        public bool HasBounds = false;

        [System.NonSerializedAttribute]
        public AnnotatableNode[] Children;

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
        public static AnnotatableNode FindChildNodes(GameObject unityObj)
        {
            var newNode = new AnnotatableNode()
            {
                Name = unityObj.name,
                Children = new AnnotatableNode[unityObj.transform.childCount],
            };
            newNode.IsAnnotatable = newNode.CheckIfObjectIsAnnotatable();

            var renderer = unityObj.GetComponent<MeshRenderer>();
            if (renderer)
            {
                newNode.Bounds = renderer.bounds;   // in worldspace
                newNode.HasBounds = true;
                newNode.HasGeometry = true;
            }


            for (int index = 0; index < unityObj.transform.childCount; index++)
            {
                var childObj = unityObj.transform.GetChild(index).gameObject;
                var childNode = FindChildNodes(childObj);
                newNode.Children[index] = childNode;

                if (childNode.HasBounds)
                {
                    if (newNode.HasBounds)
                    {
                        newNode.Bounds.Encapsulate(childNode.Bounds);
                    }
                    else
                    {
                        newNode.Bounds = childNode.Bounds;
                    }
                }
            }
            return newNode;
        }

        public void CollectChildrenIntersectingRay(Ray ray, List<AnnotatableNode> hits)
        {
            if (!this.Bounds.IntersectRay(ray))
                return;

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