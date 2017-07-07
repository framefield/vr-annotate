using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ff.vr.interaction;

namespace ff.nodegraph.interaction
{
    public class NodeSelectionMarker : MonoBehaviour
    {
        public Node _currentNode;
        public NodeSelectionManager _nodeSelectionManager;

        [SerializeField] TMPro.TextMeshPro _selectionLabel;
        [SerializeField] TMPro.TextMeshPro _parentLabel;
        [SerializeField] LaserPointerButton _exitButton;
        [SerializeField] Transform _markerQuad;

        void Awake()
        {
            _nodeSelectionManager = FindObjectOfType<NodeSelectionManager>();
        }


        void Update()
        {
            // Blend Parent/Selection labels for in/out transitions
            _dampedParentLabelPositionY = Mathf.Lerp(_dampedParentLabelPositionY, PARENT_LABEL_POS_Y, 0.1f);
            _dampedParentLabelSize = Mathf.Lerp(_dampedParentLabelSize, PARENT_LABEL_SIZE, 0.1f);
            _dampedSelectionLabelPositionY = Mathf.Lerp(_dampedSelectionLabelPositionY, SELECTION_LABEL_POS_Y, 0.1f);
            _dampedSelectionLabelSize = Mathf.Lerp(_dampedSelectionLabelSize, SELECTION_LABEL_SIZE, 0.1f);

            _parentLabel.transform.localPosition = new Vector3(0, _dampedParentLabelPositionY, 0);
            _parentLabel.transform.localScale = Vector3.one * _dampedParentLabelSize;
            _selectionLabel.transform.localPosition = new Vector3(0, _dampedSelectionLabelPositionY, 0);
            _selectionLabel.transform.localScale = Vector3.one * _dampedSelectionLabelSize;

            _dampedMarkerSize = Mathf.Lerp(_dampedMarkerSize, DEFAULT_MARKER_SIZE, 0.1f);
            _markerQuad.transform.localScale = Vector3.one * _dampedMarkerSize;
            _markerQuad.transform.Rotate(Vector3.forward, 1f);

            // Scale and orient for camera
            transform.position = Vector3.Lerp(transform.position, _targetPosition, 0.3f);
            var distance = Vector3.Distance(transform.position, Camera.main.transform.position);
            var scaleByDistance = Mathf.Sqrt(distance / 2) * DEFAULT_SIZE;
            transform.localScale = Vector3.one * scaleByDistance;

            var d = transform.position - (Camera.main.transform.position - transform.position);
            this.transform.LookAt(d);
        }


        /** Called from SelectionManager */
        public void SetCurrent(Node newNode)
        {
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
            _parentLabel.text = hasParent ? newNode.Parent.Name : "";
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

        private Vector3 _targetPosition;

        float _dampedMarkerSize = 0;

        const float SELECTION_LABEL_POS_Y = 0f;
        const float SELECTION_LABEL_SIZE = 1f;

        const float PARENT_LABEL_POS_Y = 1f;
        const float PARENT_LABEL_SIZE = 0.5f;

        float _dampedParentLabelPositionY = PARENT_LABEL_POS_Y;
        float _dampedParentLabelSize = PARENT_LABEL_SIZE;

        float _dampedSelectionLabelPositionY = SELECTION_LABEL_POS_Y;
        float _dampedSelectionLabelSize = SELECTION_LABEL_SIZE;

        const float DEFAULT_SIZE = 0.3f;
        const float DEFAULT_MARKER_SIZE = 1;

    }
}
