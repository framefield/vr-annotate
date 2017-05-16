
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ff.vr.annotation;

namespace ff.vr.annotation
{
    public class AnnotatableGroup : MonoBehaviour
    {
        public string DB_ReferenceID;

        private void Start()
        {
        }

        private void Awake()
        {
            CreateNodesFromHierachy();
        }

        private void CreateNodesFromHierachy()
        {
            var node = AddObjectToNode(this.gameObject);
            node.PrintStructure();
        }


        private AnnotatableNode AddObjectToNode(GameObject unityObj)
        {
            var newNode = new AnnotatableNode()
            {
                IsAnnotatable = CheckIfObjectIsAnnotatable(unityObj),
                Name = unityObj.name,
                Children = new AnnotatableNode[unityObj.transform.childCount],
            };

            var renderer = unityObj.GetComponent<MeshRenderer>();
            if (renderer)
            {
                newNode.Bounds = renderer.bounds;   // in worldspace
                newNode.HasBounds = true;
            }


            for (int index = 0; index < unityObj.transform.childCount; index++)
            {
                var childObj = unityObj.transform.GetChild(index).gameObject;
                var childNode = AddObjectToNode(childObj);
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


        private bool CheckIfObjectIsAnnotatable(GameObject obj)
        {
            return true;
        }
    }
}