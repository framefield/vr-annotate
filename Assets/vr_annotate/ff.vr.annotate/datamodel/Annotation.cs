using System;
using System.Collections.Generic;
using System.Text;
using ff.location;
using ff.nodegraph;
using ff.vr.annotate.viz;
using UnityEngine;
using ff.utils;
using System.Text.RegularExpressions;
using System.Globalization;
using ff.nodegraph.interaction;

namespace ff.vr.annotate.datamodel
{
    [System.Serializable]
    public class Annotation
    {
        public Annotation() { }
        public Annotation(string jsonString)
        {
            var jsonObject = new JSONObject(jsonString);
            DeserializeFromJson(jsonObject);
        }
        public Annotation(JSONObject jsonObject)
        {
            DeserializeFromJson(jsonObject);
        }

        public ID JsonLdId;

        public string LocalAnnotationsDirectory { get { return Application.dataPath + "/db/targets/"; } }

        public string AnnotationURILocal { get { return LocalAnnotationsDirectory + JsonLdId + ".json"; } }

        const string SERVER_TARGETS_URI = "http://127.0.0.1:8301/targets/";

        public string AnnotationURIRemote { get { return SERVER_TARGETS_URI + TargetNode.GUID + JsonLdId; } }



        public Person Author;
        public string Text;

        [System.NonSerializedAttribute]
        public Node TargetNode;

        public DateTime CreatedAt;
        public GeoCoordinate AnnotationPosition;

        public string ToJson()
        {
            return JsonTemplate.FillTemplate(JSONTemplate, new Dictionary<string, string>() {

                {"@id", JsonLdId.ToString()},
                {"authorJSON", JsonUtility.ToJson(Author)},
                {"createdTimestamp", CreatedAt.ToString()},
                {"annotationText", Text},
                {"targetID", TargetNode.GUID.ToString()},
                {"targetNodeName", TargetNode.Name},
                {"simulatedDate", AnnotationManager.Instance.SimulatedYear},
                {"simulatedTimeofDay", AnnotationManager.Instance.SimulatedTimeOfDay},
                {"guidPath", TargetNode.GuidPath},
                { "position",JsonUtility.ToJson(AnnotationPosition)},
            });
        }

        private string BuildAnnotatableNodeCoordinates()
        {
            string annotableNodeCoordinates = "";
            var node = TargetNode;
            while (node != null)
            {
                annotableNodeCoordinates += BuildAnnotatableNodeCoordinateForNode(node);
                node = node.Parent;
            }
            return annotableNodeCoordinates;
        }

        private string BuildAnnotatableNodeCoordinateForNode(Node annotatableNode)
        {
            var annotationPositionLocal = annotatableNode.UnityObj.transform.InverseTransformPoint(AnnotationPosition.position);
            var jsonSnippet = @"
            {
            'type':'AnnotableNodeCoordinate',
            'annotatableNodeId':'{" + annotatableNode.GUID + @"}',
            'annotationPosition':{ 'x':" + annotationPositionLocal.x + @",'y':" + annotationPositionLocal.y + @", 'z':" + annotationPositionLocal.z + @"},
            },";
            return jsonSnippet;
        }


        public void DeserializeFromJson(JSONObject jsonObject)
        {
            var id = jsonObject["id"].str;

            JsonLdId = new ID(id);

            DateTime.TryParse(jsonObject["created"].str, out CreatedAt);
            Text = jsonObject["body"][0]["value"].str;

            // Initialize Target Object
            var nodePath = jsonObject["target"]["selector"]["guidPath"].str;

            if (NodeSelector.Instance != null)
                TargetNode = NodeSelector.Instance.FindNodeFromGuidPath(nodePath);

            AnnotationPosition = JsonUtility.FromJson<GeoCoordinate>(jsonObject["target"]["position"].ToString());

            // Initialize Author
            var authorId = jsonObject["creator"]["id"].str;
            if (Person.PeopleById.ContainsKey(authorId))
            {
                Author = Person.PeopleById[authorId];
            }
            else
            {
                Author = new Person()
                {
                    id = authorId,
                    name = jsonObject["creator"]["name"].str,
                    email = jsonObject["creator"]["email"].str,
                };
                Person.PeopleById[authorId] = Author;
            }
        }

        private const string DateTimeFormat = "yyyy-MM-dd hh:mm:ss";
        private static string JSONTemplate = @"

{
    '@context': 'http://www.w3.org/ns/anno.jsonld',
    '@id': '{@id}',
    'type': 'Annotation',
    'creator': {authorJSON},
    'created': '{createdTimestamp}',
    'generator': {
        'id': 'http://vr-annotator/v/01',
        'type': 'Software',
        'name': 'VR-Annotator v0.1'
    },
    'body': [
        {
            'type': 'TextualBody',
            'purpose': 'describing',
            'value': '{annotationText}'
        }
    ],
    'target': {
        'id': '{{targetID}}',
        'type': 'http://vr-annotator/feature/',
        'targetNodeName':  '{targetNodeName}',
        'state': 
            {
                'type': 'VRSimulation',
                'refinedBy': {
                    'type': 'SimulationTime',
                    'sourceDate': '{simulatedDate}',
                    'timeOfDay': '{simulatedTimeofDay}'
                }
            },
        'selector': {
            'type': 'nodeGraphPath',
            'guidPath': '{guidPath}'
        },
        'position':  {position}
        
    }
}
";

    }
}
