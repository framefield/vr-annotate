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

        const string ANNOTATION_ID_PREFIX = "http://annotator/anno/";

        public Guid GUID;
        public string ID
        {
            get { return GUID.ToString(); }
        }
        public string JsonLdId { get { return ANNOTATION_ID_PREFIX + ID; } }
        public string Text;

        [System.NonSerializedAttribute]
        public Node TargetNode;
        public System.Guid TargetNodeId;
        public string TargetNodeName;
        public string RootNodeId;

        public Person Author;
        public GeoCoordinate ViewPointPosition;
        public GeoCoordinate AnnotationPosition;
        public DateTime CreatedAt;

        public string ToJson()
        {
            return JsonTemplate.FillTemplate(JSONTemplate, new Dictionary<string, string>() {
                {"annotationGUID", JsonLdId},
                {"authorJSON", JsonUtility.ToJson(Author)},
                {"createdTimestamp", CreatedAt.ToString()},
                {"annotationText", Text},
                {"rootNodeId", TargetNode.NodeGraphRoot.RootNodeId},
                {"targetNodeId", TargetNodeId.ToString()},
                {"targetNodeName", TargetNode.Name},
                {"simulatedDate", AnnotationManager._instance.SimulatedYear},
                {"simulatedTimeofDay", AnnotationManager._instance.SimulatedTimeOfDay},
                {"interpretationStateJSON", "{}"},
                {"sceneGraphPath", TargetNode.NodePath},
                {"viewPointPositionJSON", JsonUtility.ToJson(ViewPointPosition)},
                {"annotationPositionJSON", JsonUtility.ToJson(AnnotationPosition)},
                {"modelAuthor", TargetNode.NodeGraphRoot.modelAuthor},
                {"modelVersion", TargetNode.NodeGraphRoot.modelVersion},
            });
        }

        public void DeserializeFromJson(string jsonString)
        {
            JSONObject j = new JSONObject(jsonString);

            var uidMatchResult = new Regex(@"/(\/\d[a-f]-)/+", RegexOptions.IgnoreCase).Match(j["id"].ToString());
            if (uidMatchResult.Success)
            {
                GUID = new Guid(uidMatchResult.Groups[1].Value);
            }

            DateTime.TryParse(j["created"].str, out CreatedAt);
            Text = j["body"][0]["value"].str;

            // Initialize Target Object
            RootNodeId = j["target"]["rootNodeId"].str;
            var nodePath = j["target"]["selector"]["value"].str;
            TargetNode = NodeSelectionManager.Instance.FindNodeFromPath(RootNodeId, nodePath);

            ViewPointPosition = JsonUtility.FromJson<GeoCoordinate>(j["target"]["position"]["AnnotationViewPoint"].ToString());
            AnnotationPosition = JsonUtility.FromJson<GeoCoordinate>(j["target"]["position"]["AnnotationCoordinates"].ToString());

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

        private string JSONTemplate = @"
{
    '@context': 'http://www.w3.org/ns/anno.jsonld',
    'id': '{annotationGUID}',
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
        'type': 'http://vr-annotator/feature/',
        'rootNodeId': '{rootNodeId}',
        'targetNodeId': '{targetNodeId}',
        'targetNodeName': '{targetNodeName}',
        'state': [
            {
                'type': 'VRSimulation',
                'refinedBy': {
                    'type': 'SimulationTime',
                    'sourceDate': '{simulatedDate}',
                    'timeOfDay': '{simulatedTimeofDay}'
                }
            },
            {
                'type': 'InterpretiveReconstruction',
                'refinedBy': {interpretationStateJSON},
            },
            {
                'type': 'model',
                'refinedBy': {
                    'author' : '{modelAuthor}',
                    'version' : '{modelVersion}',
                },
            }
        ],
        'selector': {
                'type': 'SceneGraphSelector',
                'value': '{sceneGraphPath}'
        },
        'position': {
            'type':'AnnotationLocation',
            'AnnotationViewPoint': {viewPointPositionJSON},
            'AnnotationCoordinates' : {annotationPositionJSON}
        }
    }
}
";
    }
}

