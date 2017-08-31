using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace ff.nodegraph
{
    [ExecuteInEditMode]
    public class GUIDGenerator : MonoBehaviour
    {
        public static string GUIDPattern = "(\\s+)(\\[)(.{36})(\\])";

        public void GenerateGUIDsForAllChildren()
        {
            foreach (var node in GetComponentsInChildren<Transform>())
            {
                var hasID = Regex.Match(node.name, GUIDPattern).Success;
                if (!hasID)
                {
                    var id = System.Guid.NewGuid();
                    node.name = node.name + " [" + id + "]";
                }
            }
        }

        public void RemoveGUIDFromAllChildren()
        {
            foreach (var node in GetComponentsInChildren<Transform>())
            {
                node.name = RemoveGUIDFromName(node.name);
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