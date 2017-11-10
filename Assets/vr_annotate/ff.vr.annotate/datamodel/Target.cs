using System;
using System.Collections.Generic;
using System.Text;
using ff.location;
using ff.nodegraph;
using ff.vr.annotate.viz;
using ff.vr.annotate.datamodel;
using UnityEngine;
using ff.utils;
using System.Text.RegularExpressions;
using System.Globalization;
using ff.nodegraph.interaction;
using System.IO;
using System.Collections;
using UnityEngine.Networking;

namespace ff.nodegraph
{
    [System.Serializable]
    public class Target : NodeGraph
    {
        public Target() { }

        public Target(JSONObject jSONObject)
        {
            DeserializeFromJson(jSONObject);
        }

        public LinkedDataID JsonLdId
        {
            get
            {
                return new LinkedDataID(LinkedDataID.IDType.Target, Node.GUID);
            }
        }

        public DataBaseLocation DataBaseLocationToUse;

        public enum DataBaseLocation
        {
            REST,
            localDirectory
        }

        void Start()
        {
            SyncWithDataBase();
        }

        public void SyncWithDataBase()
        {
            switch (DataBaseLocationToUse)
            {
                case DataBaseLocation.localDirectory:
                    Serialization.SyncWithLocalDirectory(this);
                    break;
                case DataBaseLocation.REST:
                    StartCoroutine(Serialization.SyncWithServerCoroutine(this));
                    break;
            }
        }

        public string ToJson()
        {
            return JsonTemplate.FillTemplate(JSONTemplate, new Dictionary<string, string>() {
                {"@id",  JsonLdId.ToString()},
                {"nodeGraph", SerializeNodeTreeRecursive(Node).Replace("'", "\"")},
            });
        }
        private string SerializeNodeTreeRecursive(Node currentNode)
        {
            var json = "{ \"nodeId\" : \" " + currentNode.GUID.ToString() + " \",  \"children\" : [";

            for (int i = 0; i < currentNode.Children.Length; i++)
            {
                if (i != 0)
                    json += ",";

                json += SerializeNodeTreeRecursive(currentNode.Children[i]);
            }
            json += "]}";
            return json;
        }

        public static Target DeserializeFromJson(JSONObject jsonObject)
        {
            var newTarget = new Target();
            newTarget.GUID = new Guid(jsonObject["@id"].str);
            newTarget.Node = DeserializeGraph(jsonObject["nodeGraph"]);
            return newTarget;
        }

        private static Node DeserializeGraph(JSONObject jsonObject)
        {
            var newNode = new Node();

            var GUIDString = jsonObject["nodeId"].str;
            newNode.GUID = new Guid(GUIDString);

            var children = new List<Node>();
            foreach (var childJsonNode in jsonObject["children"])
            {
                children.Add(DeserializeGraph(childJsonNode));
            }
            newNode.Children = children.ToArray();

            return newNode;
        }

        public static string JSONTemplate = @"
        {
            '@context': 
            {
                '@vocab':'http://www.w3.org/ns/target.jsonld',
                '@base': 'http://annotator/target/'
            },
            'id' : '{@id}',
            '@type': 'AnnotationTarget',
            'creator': {'id':'_alan','name':'Alan','email':'alan @google.com'},
            'created': '7/10/2017 7:02:44 PM',
            'generator': 
            {
                'id': 'http://vr-annotator/v/02',
                'type': 'Software',
                'name': 'VR-Annotator v0.2'
            },
            'interpretation': 
            {
                'refinedBy': 
                {
                    'modellerName':'',
                    'modellingSoftware':'',
                    'references': []
                }
            },
            'nodeGraph': {nodeGraph}
        }";
    }
}
