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
            DeserializeFromJson(jsonString);
        }

        public Guid Guid;
        const string ANNOTATION_ID_PREFIX = "http://annotator/anno/";
        public string JsonLdId { get { return ANNOTATION_ID_PREFIX + Guid; } }
        public Person Author;
        public string Text;

        [System.NonSerializedAttribute]
        public Node TargetNode;
        public System.Guid TargetNodeId;
        public string TargetNodeName;
        public string RootNodeId;

        public AnnotationAndViewPortPosition AnnotationFrame;
        public DateTime CreatedAt;

        public struct AnnotationAndViewPortPosition
        {
            public GeoCoordinate AnnotationPosition;
            public GeoCoordinate ViewPortPosition;
        }

        public string ToJson()
        {

            return JsonTemplate.FillTemplate(JSONTemplate, new Dictionary<string, string>() {

                {"annotationGUID", JsonLdId},
                {"authorJSON", JsonUtility.ToJson(Author)},
                {"createdTimestamp", CreatedAt.ToString()},
                {"annotationText", Text},
                {"targetID", TargetNodeId.ToString()},
                {"targetNodeName", TargetNode.Name},
                {"simulatedDate", AnnotationManager.Instance.SimulatedYear},
                {"simulatedTimeofDay", AnnotationManager.Instance.SimulatedTimeOfDay},
                // {"interpretationStateJSON", "{}"},
                {"guidPath", TargetNode.GuidPath},
                { "annotationLatitude",AnnotationFrame.AnnotationPosition.latitude.ToString()},
                { "annotationLongitude",AnnotationFrame.AnnotationPosition.longitude.ToString()},
                { "annotationElevation",AnnotationFrame.AnnotationPosition.elevation.ToString()},
                { "viewportLatitude", AnnotationFrame.ViewPortPosition.latitude.ToString()},
                { "viewportLongitude", AnnotationFrame.ViewPortPosition.longitude.ToString()},
                { "viewportElevation", AnnotationFrame.ViewPortPosition.elevation.ToString()},
                { "AnnotableNodeCoordinates",BuildAnnotatableNodeCoordinates()},
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
            var annotationPositionLocal = annotatableNode.UnityObj.transform.InverseTransformPoint(AnnotationFrame.AnnotationPosition.position);
            var viewPortPositionLocal = annotatableNode.UnityObj.transform.InverseTransformPoint(AnnotationFrame.ViewPortPosition.position);
            var jsonSnippet = @"
            {
            'type':'AnnotableNodeCoordinate',
            'annotatableNodeId':'{" + annotatableNode.GUID + @"}',
            'annotationPosition':{ 'x':" + annotationPositionLocal.x + @",'y':" + annotationPositionLocal.y + @", 'z':" + annotationPositionLocal.z + @"},
            'viewPortPosition':{ 'x':" + viewPortPositionLocal.x + @",'y':" + viewPortPositionLocal.y + @", 'z':" + viewPortPositionLocal.z + @"},
            },";
            return jsonSnippet;
        }


        public void DeserializeFromJson(string jsonString)
        {
            JSONObject j = new JSONObject(jsonString);

            var uidMatchResult = new Regex(@"/(\/\d[a-f]-)/+", RegexOptions.IgnoreCase).Match(j["@id"].ToString());
            if (uidMatchResult.Success)
            {
                Guid = new Guid(uidMatchResult.Groups[1].Value);
            }

            DateTime.TryParse(j["created"].str, out CreatedAt);
            Text = j["body"][0]["value"].str;

            // Initialize Target Object
            var nodePath = j["target"]["selector"]["guidPath"].str;


            if (NodeSelector.Instance != null)
                TargetNode = NodeSelector.Instance.FindNodeFromGuidPath(nodePath);

            var viewportPosition = new GeoCoordinate();


            AnnotationFrame.ViewPortPosition = JsonUtility.FromJson<GeoCoordinate>(j["target"]["position"]["AnnotationViewPort"].ToString());
            AnnotationFrame.AnnotationPosition = JsonUtility.FromJson<GeoCoordinate>(j["target"]["position"]["AnnotationCoordinates"].ToString());

            // Initialize Author
            var authorId = j["creator"]["id"].str;
            if (Person.PeopleById.ContainsKey(authorId))
            {
                Author = Person.PeopleById[authorId];
            }
            else
            {
                Author = new Person()
                {
                    id = authorId,
                    name = j["creator"]["name"].str,
                    email = j["creator"]["email"].str,
                };
                Person.PeopleById[authorId] = Author;
            }
            // Find Object Reference 
        }

        private const string DateTimeFormat = "yyyy-MM-dd hh:mm:ss";

        private static string JSONTemplate = @"

{
    '@context': 'http://www.w3.org/ns/anno.jsonld',
    '@id': '{annotationGUID}',
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
        'state': [
            {
                'type': 'VRSimulation',
                'refinedBy': {
                    'type': 'SimulationTime',
                    'sourceDate': '{simulatedDate}',
                    'timeOfDay': '{simulatedTimeofDay}'
                }
            },
        ],
        'selector': {
                'type': 'nodeGraphPath',
                'value': 'Stoa_komplett_low/Nordanbau/Phase_2_3',
                'guidPath': '{guidPath}',
            }
        },

        'annotationCoordiantes':[
        {
            'type':'GeoCoordinates',          
            'annotationLatitude':'{annotationLatitude}',
            'annotationLongitude':'{annotationLongitude}',
            'annotationElevation':'{annotationElevation}',
            'viewportLatitude':'{viewportLatitude}',
            'viewportLongitude':'{viewportLongitude}',
            'viewportElevation':'{viewportElevation}'
        },
        '{AnnotableNodeCoordinates}'                  
        ],    
    }
}
";


    }
}
