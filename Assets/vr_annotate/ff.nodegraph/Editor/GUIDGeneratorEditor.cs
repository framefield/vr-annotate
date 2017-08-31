using UnityEngine;
using System.Collections;
using UnityEditor;

namespace ff.nodegraph
{
    [CustomEditor(typeof(GUIDGenerator))]
    public class GUIDGeneratorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            GUIDGenerator myScript = (GUIDGenerator)target;
            if (GUILayout.Button("GenerateGUIDsForAllChildren"))
            {
                myScript.GenerateGUIDsForAllChildren();
            }
            if (GUILayout.Button("RemoveGUIDFromAllChildren"))
            {
                myScript.RemoveGUIDFromAllChildren();
            }
        }
    }
}