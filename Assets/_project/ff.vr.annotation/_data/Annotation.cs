using System;
using System.Collections.Generic;
using System.Text;
using ff.location;
using ff.nodegraph;
using ff.vr.annotate.viz;
using UnityEngine;
using ff.utils;

namespace ff.vr.annotate
{
    [System.Serializable]
    public class Annotation
    {
        const string ANNOTATION_ID_PREFIX = "http://annotator/anno/";

        public Guid GUID;
        public string ID
        {
            // Return short version as base64 string
            // see https://stackoverflow.com/a/9279005
            get { return GUID.ToString(); }
        }
        public string JsonLdId { get { return ANNOTATION_ID_PREFIX + ID; } }
        public Vector3 Position;
        public System.Guid ContextNodeId;
        public string Text;

        [System.NonSerializedAttribute]
        public Node ContextNode;
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
                {"annotationTargetId", ContextNodeId.ToString()},
                {"simulatedTime", AnnotationManager._instance.SimulatedTimeOfDay},
                {"simulatedTimeofDay", AnnotationManager._instance.SimulatedTimeOfDay},
                {"interpretationStateJSON", "{}"},
                {"sceneGraphPath", "to be implemented"},
                {"viewPointPositionJSON", JsonUtility.ToJson(ViewPointPosition)},
                {"annotationPositionJSON", JsonUtility.ToJson(AnnotationPosition)},
            });
        }

        public void InitFromJson()
        {

        }
        private string JSONTemplate = @"
{
    '@context': 'http://www.w3.org/ns/anno.jsonld',
    'id': 'http://annotator/anno/{annotationGUID}',
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
        'target': '{annotationTargetId}',
        'state': [
            {
                'type': 'VRSimulation',
                'refinedBy': {
                    'type': 'SimulationTime',
                    'sourceDate': '{simulatedTime}',
                    'timeOfDay': '{simulatedTimeofDay}'
                }
            },
            {
                'type': 'InterpretiveReconstruction',
                'refinedBy': {interpretationStateJSON}
            }
        ],
        'selector': {
                'type': 'SceneGraphSelector',
                'value': '{sceneGraphPath}',
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

