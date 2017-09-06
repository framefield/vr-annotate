using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ff.vr.interaction;
using System;
using ff.vr.annotate.viz;

namespace ff.nodegraph.interaction
{
    public class NodeSelectionMarker : MonoBehaviour
    {
        public Node _currentNode;
        public NodeSelector _nodeSelector;

        [SerializeField] TMPro.TextMeshPro _selectionLabel;
        [SerializeField] TMPro.TextMeshPro[] _parentLabel;
        [SerializeField] Transform _markerQuad;
        [SerializeField] Transform[] _toolIcons;
        [SerializeField] GameObject _commentIcon;

        void Awake()
        {
            _nodeSelector = FindObjectOfType<NodeSelector>();
        }

        void Start()
        {
            if (SelectionManager.Instance == null)
            {
                throw new UnityException("NodeSelectManager Requires a SelectionManager to be initialized. Are you missing an instance of SelectionManager or is the script execution order incorrect?");
            }
            SelectionManager.Instance.OnSelectedAnnotationGizmoChanged += AnnotationSelectionChangedHandler;
            SelectionManager.Instance.OnSelectedNodeChanged += NodeSelectionChangedHandler;
            SelectionManager.Instance.OnNodeSelectionMarkerPositionChanged += NodeSelectionMarkerPositionChangedHandler;
            SelectionManager.Instance.OnSelectedAnnotationGizmoChanged += AnnotationGizmoSelectionChangedHandler;
        }

        private void AnnotationGizmoSelectionChangedHandler(AnnotationGizmo obj)
        {
            _commentIcon.SetActive(obj == null);
        }

        void Update()
        {
            // Blend Parent/Selection labels for in/out transitions
            _dampedParentLabelPositionY = Mathf.Lerp(_dampedParentLabelPositionY, PARENT_LABEL_POS_Y, BLEND_SPEED);
            _dampedParentLabelSize = Mathf.Lerp(_dampedParentLabelSize, PARENT_LABEL_SIZE, BLEND_SPEED);
            _dampedSelectionLabelPositionY = Mathf.Lerp(_dampedSelectionLabelPositionY, SELECTION_LABEL_POS_Y, BLEND_SPEED);
            _dampedSelectionLabelSize = Mathf.Lerp(_dampedSelectionLabelSize, SELECTION_LABEL_SIZE, BLEND_SPEED);

            //todo: fix fade in
            // _parentLabel.transform.localPosition = new Vector3(0.5f, _dampedParentLabelPositionY, 0);
            // _parentLabel.transform.localScale = Vector3.one * _dampedParentLabelSize;
            // _selectionLabel.transform.localPosition = new Vector3(0.5f, _dampedSelectionLabelPositionY, 0);
            // _selectionLabel.transform.localScale = Vector3.one * _dampedSelectionLabelSize;

            _dampedMarkerSize = Mathf.Lerp(_dampedMarkerSize, DEFAULT_MARKER_SIZE, BLEND_SPEED);
            _markerQuad.transform.localScale = Vector3.one * _dampedMarkerSize;
            _markerQuad.transform.Rotate(Vector3.forward, 1f);

            // Scale and orient for camera
            transform.position = Vector3.Lerp(transform.position, _targetPosition, 0.3f);
            utils.Helpers.FaceCameraAndKeepSize(this.transform, DEFAULT_SIZE);

            var progress = (Time.time - _selectionTime) / TRANSITION_DURATION;

            for (int index = 0; index < _toolIcons.Length; index++)
            {
                var item = _toolIcons[index];
                var smoothed = Mathf.SmoothStep(0, 1, progress - index * 0.2f);
                item.transform.localScale = Vector3.one * smoothed * DEFAULT_ICON_SIZE;
                item.transform.localPosition = Vector3.left * smoothed * (index + 1.2f) * DEFAULT_ICON_SIZE;
            }
            WriteParentButtons();
        }

        private void AnnotationSelectionChangedHandler(AnnotationGizmo newGizmo)
        {
            if (newGizmo == null)
                return;
            var newNode = newGizmo.Annotation.TargetNode;
            PrepareLabelTransition(newNode);

            var isValid = newNode != null;
            this.gameObject.SetActive(isValid);
            _selectionLabel.text = isValid ? newNode.Name : "";
            WriteParentButtons();

            _currentNode = newNode;
        }

        private void NodeSelectionChangedHandler(Node node)
        {
            PrepareLabelTransition(node);

            var isValid = node != null;
            this.gameObject.SetActive(isValid);
            _selectionLabel.text = isValid ? node.Name : "";
            WriteParentButtons();

            _currentNode = node;
        }
        private void WriteParentButtons()
        {
            Node parentNode = _currentNode;
            for (int i = 0; i < _parentLabel.Length; i++)
            {
                if (parentNode != null)
                    parentNode = parentNode.Parent;

                var label = _parentLabel[i];
                if (parentNode != null)
                {
                    label.text = parentNode.Name + " /";
                    label.gameObject.SetActive(true);
                }
                else
                {
                    label.GetComponent<TMPro.TextMeshPro>().text = "";
                    label.gameObject.SetActive(false);
                }
            }
        }

        private void PrepareLabelTransition(Node newNode)
        {
            _selectionTime = Time.time;
            if (_currentNode != null && newNode == _currentNode.Parent)
            {
                _dampedParentLabelPositionY = 2 * PARENT_LABEL_POS_Y;
                _dampedParentLabelSize = 0;

                _dampedSelectionLabelPositionY = PARENT_LABEL_POS_Y;
                _dampedSelectionLabelSize = PARENT_LABEL_SIZE;
            }
            else if (_currentNode != null && _currentNode.Children != null)
            {
                foreach (var child in _currentNode.Children)
                {
                    if (newNode == child)
                    {
                        _dampedParentLabelPositionY = SELECTION_LABEL_POS_Y;
                        _dampedParentLabelSize = SELECTION_LABEL_SIZE;

                        _dampedSelectionLabelPositionY = -PARENT_LABEL_POS_Y;
                        _dampedSelectionLabelSize = 0;
                        break;
                    }
                }
            }
            transform.localScale = Vector3.zero;
        }

        // is called from Button
        public void CreateAnnotationAtCurrentNode()
        {
            AnnotationManager.Instance.CreateAnnotation(SelectionManager.Instance.SelectedNode, _targetPosition);
        }

        public void NodeSelectionMarkerPositionChangedHandler(Vector3 newPosition)
        {
            _targetPosition = newPosition;
            _dampedMarkerSize = 2;
        }

        public Vector3 GetPosition()
        {
            return transform.position;
        }

        private Vector3 _targetPosition;

        float _dampedMarkerSize = 0;

        const float SELECTION_LABEL_POS_Y = 0f;
        const float SELECTION_LABEL_SIZE = 1f;

        const float PARENT_LABEL_POS_Y = 0.6f;
        const float PARENT_LABEL_SIZE = 0.5f;

        float _dampedParentLabelPositionY = PARENT_LABEL_POS_Y;
        float _dampedParentLabelSize = PARENT_LABEL_SIZE;

        float _dampedSelectionLabelPositionY = SELECTION_LABEL_POS_Y;
        float _dampedSelectionLabelSize = SELECTION_LABEL_SIZE;

        const float DEFAULT_SIZE = 0.3f;
        const float DEFAULT_MARKER_SIZE = 0.5f;
        const float DEFAULT_ICON_SIZE = 0.75f;
        private float _selectionTime;
        private const float TRANSITION_DURATION = 0.5f;

        const float BLEND_SPEED = 0.08f;
    }
}
