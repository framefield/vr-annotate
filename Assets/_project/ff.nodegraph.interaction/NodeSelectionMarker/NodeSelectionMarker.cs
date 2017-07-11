using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ff.vr.interaction;
using System;

namespace ff.nodegraph.interaction
{
    public class NodeSelectionMarker : MonoBehaviour, ISelectable
    {
        public Node _currentNode;
        public NodeSelectionManager _nodeSelectionManager;

        [SerializeField] TMPro.TextMeshPro _selectionLabel;
        [SerializeField] TMPro.TextMeshPro _parentLabel;
        [SerializeField] LaserPointerButton _exitButton;
        [SerializeField] Transform _markerQuad;
        [SerializeField] Transform[] _toolIcons;

        void Awake()
        {
            _nodeSelectionManager = FindObjectOfType<NodeSelectionManager>();
            _infoPanel = FindObjectOfType<InformationPanel>();
        }

        private InformationPanel _infoPanel;

        const float BLEND_SPEED = 0.08f;

        void Update()
        {
            // Blend Parent/Selection labels for in/out transitions
            _dampedParentLabelPositionY = Mathf.Lerp(_dampedParentLabelPositionY, PARENT_LABEL_POS_Y, BLEND_SPEED);
            _dampedParentLabelSize = Mathf.Lerp(_dampedParentLabelSize, PARENT_LABEL_SIZE, BLEND_SPEED);
            _dampedSelectionLabelPositionY = Mathf.Lerp(_dampedSelectionLabelPositionY, SELECTION_LABEL_POS_Y, BLEND_SPEED);
            _dampedSelectionLabelSize = Mathf.Lerp(_dampedSelectionLabelSize, SELECTION_LABEL_SIZE, BLEND_SPEED);

            _parentLabel.transform.localPosition = new Vector3(0.5f, _dampedParentLabelPositionY, 0);
            _parentLabel.transform.localScale = Vector3.one * _dampedParentLabelSize;
            _selectionLabel.transform.localPosition = new Vector3(0.5f, _dampedSelectionLabelPositionY, 0);
            _selectionLabel.transform.localScale = Vector3.one * _dampedSelectionLabelSize;

            _dampedMarkerSize = Mathf.Lerp(_dampedMarkerSize, DEFAULT_MARKER_SIZE, BLEND_SPEED);
            _markerQuad.transform.localScale = Vector3.one * _dampedMarkerSize;
            _markerQuad.transform.Rotate(Vector3.forward, 1f);

            // Scale and orient for camera
            transform.position = Vector3.Lerp(transform.position, _targetPosition, 0.3f);
            utils.Helpers.FaceCameraAndKeepSize(this.transform, DEFAULT_SIZE);

            var s = _exitButton.transform.localScale;
            s.x = (_parentLabel.renderedWidth + 0.2f) / 2.0f;
            _exitButton.transform.localScale = s;

            var p = _exitButton.transform.localPosition;
            p.x = (s.x * 0.5f) + 0.5f - 0.05f;
            _exitButton.transform.localPosition = p;

            var progress = (Time.time - _selectionTime) / TRANSITION_DURATION;

            for (int index = 0; index < _toolIcons.Length; index++)
            {
                var item = _toolIcons[index];
                var smoothed = Mathf.SmoothStep(0, 1, progress - index * 0.2f);
                item.transform.localScale = Vector3.one * smoothed * DEFAULT_ICON_SIZE;
                item.transform.localPosition = Vector3.left * smoothed * (index + 1.2f) * DEFAULT_ICON_SIZE;
            }
        }


        /** Called from SelectionManager */
        public void SetCurrent(Node newNode)
        {
            _selectionTime = Time.time;
            if (newNode == _currentNode.Parent)
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

            var isValid = newNode != null;
            var hasParent = newNode != null && newNode.Parent != null;

            transform.localScale = Vector3.zero;

            _selectionLabel.text = isValid ? newNode.Name : "";
            _parentLabel.text = hasParent ? newNode.Parent.Name + " /" : "";
            _exitButton.gameObject.SetActive(hasParent);

            _currentNode = newNode;
        }



        public void SetPosition(Vector3 newPosition)
        {
            _targetPosition = newPosition;
            _dampedMarkerSize = 2;
        }


        /** 
            Called when clicking icon 
            SelectionManager will then call SetCurrent()
        */
        public void ExitToParent()
        {
            _nodeSelectionManager.SelectParent();
        }



        public void OnShowInformation()
        {
            if (_infoPanel)
                _infoPanel.SetSelection(this);
        }

        public void OnSelected()
        {
            Debug.LogWarning("NodeSelectionMarker should be be selectable", this);
            return;
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
    }
}
