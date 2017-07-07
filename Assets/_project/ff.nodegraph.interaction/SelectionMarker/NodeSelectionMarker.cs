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

        void Awake()
        {
            _nodeSelectionManager = FindObjectOfType<NodeSelectionManager>();
        }

        void Update()
        {
            _dampedSize = Mathf.Lerp(_targetSize, _dampedSize, 0.9f);
            this.transform.position = Vector3.Lerp(_targetPosition, transform.position, 0.7f);

            var distance = Vector3.Distance(transform.position, Camera.main.transform.position);
            var scaleByDistance = Mathf.Sqrt(distance / 2) * _dampedSize * 0.1f;
            transform.localScale = Vector3.one * scaleByDistance;

            // Face camera;
            var d = transform.position - (Camera.main.transform.position - transform.position);
            this.transform.LookAt(d);
        }

        float _dampedSize = 0;


        /** Called from SelectionManager */
        public void SetCurrent(Node newNode)
        {
            var hasParent = newNode != null && newNode.Parent != null;

            var isValid = newNode != null;


            _targetSize = isValid ? 0.4f : 0.01f;
            transform.localScale = Vector3.zero;

            _selectionLabel.text = isValid ? newNode.Name : "";
            _parentLabel.text = hasParent ? newNode.Parent.Name : "";
            _exitButton.gameObject.SetActive(hasParent);

            _currentNode = newNode;
        }



        public void SetPosition(Vector3 newPosition)
        {



            _targetPosition = newPosition;
            _targetSize = 2;
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
        private float _targetSize;
    }
}
