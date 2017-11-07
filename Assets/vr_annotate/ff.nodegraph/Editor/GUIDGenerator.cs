using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace ff.nodegraph
{
    [ExecuteInEditMode]
    public class GUIDGenerator : MonoBehaviour
    {
        [MenuItem("vr-annotate/Generate GUIDs for all Children")]
        public static void GenerateGUIDsForAllChildren()
        {
            NodeGraph[] nodeGraphs = FindObjectsOfType(typeof(NodeGraph)) as NodeGraph[];
            foreach (var ng in nodeGraphs)
            {
                foreach (var node in ng.GetComponentsInChildren<Transform>())
                {
                    var hasID = Regex.Match(node.name, GUIDHelper.GUIDPattern).Success;
                    if (!hasID)
                    {
                        var id = System.Guid.NewGuid();
                        node.name = node.name + " [" + id + "]";
                    }
                }
            }
        }

        [MenuItem("vr-annotate/Remove GUID from all Children")]
        public static void RemoveGUIDFromAllChildren()
        {
            NodeGraph[] nodeGraphs = FindObjectsOfType(typeof(NodeGraph)) as NodeGraph[];
            foreach (var ng in nodeGraphs)
            {
                foreach (var node in ng.GetComponentsInChildren<Transform>())
                {
                    node.name = GUIDHelper.RemoveGUIDFromName(node.name);
                }
            }
        }

        [MenuItem("vr-annotate/Synchronize all Targets with DataBase")]
        public static void SyncAllTargetsWithServer()
        {
            Target[] targets = FindObjectsOfType(typeof(Target)) as Target[];
            foreach (var t in targets)
            {
                t.SyncWithDataBase();
            }
        }
    }
}