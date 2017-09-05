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
        public static string GUIDPattern = "(\\s+)(\\[)(.{36})(\\])";

        [MenuItem("vr-annotate/Generate GUIDs for all Children")]
        public static void GenerateGUIDsForAllChildren()
        {
            NodeGraph[] nodeGraphs = FindObjectsOfType(typeof(NodeGraph)) as NodeGraph[];
            foreach (var ng in nodeGraphs)
            {
                foreach (var node in ng.GetComponentsInChildren<Transform>())
                {
                    var hasID = Regex.Match(node.name, GUIDPattern).Success;
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
                    node.name = RemoveGUIDFromName(node.name);
                }
            }
        }

        public static String ExtractGUIDFromName(string name)
        {
            var match = Regex.Match(name, GUIDPattern);
            if (match.Success)
            {
                var guidString = match.Captures[0].Value;
                guidString = Regex.Replace(guidString, "(\\[|\\])", "");
                return guidString;
            }
            else
                return ErrorGUID.ToString();
        }

        public static String RemoveGUIDFromName(string name)
        {
            return Regex.Replace(name, GUIDPattern, "");
        }

        private static Guid ErrorGUID = new Guid();
    }
}