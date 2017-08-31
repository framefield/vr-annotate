using System.Collections;
using System.Collections.Generic;
using ff.nodegraph;
using ff.nodegraph.interaction;
using ff.vr.annotate;
using UnityEngine;


namespace ff.vr.interaction
{

    public interface IInfoPanelContent
    {
        void ForwardSelectionFromInfoPanel(ISelectable newSelection);
    }

    public class InfoPanel : MonoBehaviour
    {
        [SerializeField]
        NodeGraphOutliner _contentForNodes;

        [SerializeField]
        InfoPanelContentForAnnotations _contentForAnnotations;


        [Header("--- internal prefab references -----")]
        [SerializeField]
        LineRenderer _connectionLine;


        void Start()
        {
            if (_instance != null)
            {
                throw new UnityException("" + this + " can only be added once");
            }
            _instance = this;

            foreach (SteamVR_TrackedController controller in Resources.FindObjectsOfTypeAll(typeof(SteamVR_TrackedController)))
            {
                controller.MenuButtonClicked += MenuButtonClickedHandler;
                controller.MenuButtonUnclicked += MenuButtonUnclickedHandler;
            }

            _selectionMarker = GameObject.FindObjectOfType<NodeSelectionMarker>();
            if (_selectionMarker == null)
            {
                Debug.LogWarning("" + this + " requires an instance of NodeSelectionMarker within the scene to render connection line.", this);
            }

            if (SelectionManager.Instance == null)
            {
                throw new UnityException("" + this + " requires a SelectionManager to be initialized. Are you missing an instance of SelectionManager or is the script execution order incorrect?");
            }
            SelectionManager.Instance.SelectedNodeChangedEvent += NodeSelectionChangedHandler;
            SelectionManager.Instance.SelectedAnnotationGizmoChangedEvent += GizmoSelectionChangedHandler;
        }

        private bool forceMoveIntoView;
        private void NodeSelectionChangedHandler(Node selectedNode)
        {
            _selectedItem = selectedNode;
            if (_selectedItem == null)
                return;

            if (!IsVisibleInView || forceMoveIntoView)
                MoveIntoView();

            _contentForAnnotations.gameObject.SetActive(false);
            _contentForNodes.gameObject.SetActive(true);

            _contentForNodes.ForwardSelectionFromInfoPanel(_selectedItem);
            _content = _contentForNodes;
        }


        private void GizmoSelectionChangedHandler(AnnotationGizmo selectedGizmo)
        {
            _selectedItem = selectedGizmo;
            if (_selectedItem == null)
                return;

            if (!IsVisibleInView || forceMoveIntoView)
                MoveIntoView();

            _contentForNodes.gameObject.SetActive(false);
            _contentForAnnotations.gameObject.SetActive(true);

            _contentForAnnotations.ForwardSelectionFromInfoPanel(_selectedItem);

            var annotation = selectedGizmo.Annotation;
            var annotatedNode = annotation != null ? annotation.TargetNode : null;

            _contentForNodes.ForwardSelectionFromInfoPanel(annotatedNode);
            _content = _contentForAnnotations;

        }


        void Update()
        {
            var transitionComplete = TransitionProgress == 1;

            UpdateConnectionLine();

            switch (_state)
            {
                case States.Undefined:
                    break;

                case States.Closing:
                    if (transitionComplete)
                    {
                        SetState(States.Closed);
                    }
                    transform.localScale = Vector3.one * (1 - TransitionProgress);
                    break;

                case States.Closed:
                    break;

                case States.Opening:
                    if (transitionComplete)
                    {
                        SetState(States.Open);
                    }

                    this.transform.localScale = Vector3.one * TransitionProgress;
                    if (_pressedMenuButtonController != null)
                    {
                        transform.position = PositionFromController;
                        transform.rotation = RotationFromController;
                    }
                    break;
                case States.Open:
                    if (_pressedMenuButtonController != null)
                    {
                        transform.position = PositionFromController;
                        transform.rotation = RotationFromController;
                    }

                    break;

                case States.MovingIntoView:
                    if (transitionComplete)
                    {
                        SetState(States.Open);
                    }
                    this.transform.position = Vector3.Lerp(transform.position, PositionFromController, 0.1f);
                    this.transform.rotation = Quaternion.Lerp(transform.rotation, RotationFromController, 0.1f);
                    break;
            }
        }


        private void MenuButtonClickedHandler(object sender, ClickedEventArgs clickedEventArgs)
        {
            var controller = sender as SteamVR_TrackedController;
            if (controller == null)
                return;

            if (_pressedMenuButtonController != null)
            {
                Debug.LogWarning("Ignoring inconsistent menu button presses", this);
                return;
            }

            _pressedMenuButtonController = controller;

            switch (_state)
            {
                case States.Undefined:
                case States.Closing:
                case States.Closed:
                    SetState(States.Opening);
                    break;
            }
        }

        private void UpdateConnectionLine()
        {
            var selectionLineVisible = _selectedItem != null;

            _connectionLine.gameObject.SetActive(selectionLineVisible);
            if (!selectionLineVisible)
                return;

            _connectionLine.SetPosition(0, _connectionLine.transform.TransformPoint(Vector3.zero));

            if (_selectedItem is Node && _selectionMarker != null)
            {
                _connectionLine.SetPosition(1, _selectionMarker.transform.position);
            }
            else
            {
                _connectionLine.SetPosition(1, _selectedItem.GetPosition());
            }

            _connectionLine.SetPosition(0, _contentForNodes.GetConnectionLineStart());
        }

        private const float CLOSE_MENU_THRESHOLD_DISTANCE = 0.3f;

        private bool IsVisibleInView
        {
            get
            {
                var screenPoint = Camera.main.WorldToViewportPoint(this.transform.position);
                var visibleInView = (screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1);
                return visibleInView;
            }
        }


        private void MenuButtonUnclickedHandler(object sender, ClickedEventArgs clickedEventArgs)
        {
            var controller = sender as SteamVR_TrackedController;
            if (controller == null)
                return;

            if (_pressedMenuButtonController != controller)
            {
                Debug.LogWarning("Ignoring inconsistent menu button release", this);
                return;
            }


            var distanceToCurrentPos = Vector3.Distance(PositionFromController, transform.position);
            var toggle = (distanceToCurrentPos < CLOSE_MENU_THRESHOLD_DISTANCE);
            switch (_state)
            {
                case States.Opening:
                case States.Open:
                case States.MovingIntoView:
                    if (toggle)
                    {
                        SetState(States.Closing);
                    }
                    else
                    {
                        SetState(States.MovingIntoView);
                    }
                    break;
            }

            _pressedMenuButtonController = null;
        }



        private void SetState(States newState)
        {
            if (newState == _state)
                return;

            var transitionActive = TransitionProgress > 0.1f && TransitionProgress < 0.9f;
            var startNewTransition = false;

            switch (newState)
            {
                case States.Undefined:
                    break;

                case States.Closing:
                    startNewTransition = true;
                    break;

                case States.Closed:
                    _connectionLine.gameObject.SetActive(false);
                    break;

                case States.Opening:
                    startNewTransition = true;
                    break;

                case States.Open:
                    _connectionLine.gameObject.SetActive(true);
                    break;

                case States.MovingIntoView:
                    startNewTransition = true;
                    break;
            }

            if (startNewTransition)
            {
                _interactionStartTime = transitionActive
                        ? Time.time - (1 - TransitionProgress)
                        : Time.time;

            }
            _state = newState;
        }


        private const float DISTANCE_FROM_CAMERA = 1.5f;
        private const float OFFSET_TO_RIGHT = 0.5f;
        public void MoveIntoView()
        {
            SetState(States.MovingIntoView);

            _lastValidPosition = Camera.main.transform.position
                + Camera.main.transform.forward * DISTANCE_FROM_CAMERA
                + Camera.main.transform.right * OFFSET_TO_RIGHT;
            var ea = Camera.main.transform.eulerAngles;
            ea.z = 0;
            ea.y += 30;
            //ea.x += 10;
            var rot = Quaternion.Euler(ea);
            _lastValidRotation = rot;

        }


        private Vector3 PositionFromController
        {
            get
            {
                if (_pressedMenuButtonController != null)
                    _lastValidPosition = _pressedMenuButtonController.transform.position + _pressedMenuButtonController.transform.forward;

                return _lastValidPosition;
            }
        }

        private Vector3 _lastValidPosition;
        private Quaternion _lastValidRotation;
        private Quaternion RotationFromController
        {
            get
            {
                if (_pressedMenuButtonController != null)
                {

                    var ea = _pressedMenuButtonController.transform.eulerAngles;
                    ea.z = 0;
                    ea.x += 30;
                    var rot = Quaternion.Euler(ea);
                    _lastValidRotation = _pressedMenuButtonController.transform.rotation = rot;
                }
                return _lastValidRotation;
            }
        }

        private float TransitionProgress
        {
            get { return Mathf.Clamp01((Time.time - _interactionStartTime) / TRANSITION_DURATION); }
        }


        [Header("--- debug only ------------")]
        [SerializeField]
        private States _state;
        private enum States
        {
            Undefined,
            Closing,
            Closed,
            Opening,
            Open,
            MovingIntoView,
        }

        SteamVR_TrackedController _pressedMenuButtonController;

        private ISelectable _selectedItem;
        private IInfoPanelContent _content;

        private const float TRANSITION_DURATION = 0.35f;
        private float _interactionStartTime = 0;

        private NodeSelectionMarker _selectionMarker;
        public static InfoPanel _instance;
    }
}
