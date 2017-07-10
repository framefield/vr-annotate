using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ff.vr.interaction
{
    public class InformationPanel : MonoBehaviour
    {
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
                controller.MenuButtonClicked += TriggerClickedHandler;
                controller.MenuButtonUnclicked += TriggerUnclickedHandler;
            }
        }


        public void SetSelection(ISelectable item)
        {
            _selectedItem = item;
            item.OnSelected();
            SetState(States.MovingIntoView);
        }


        void Update()
        {
            var transitionComplete = TransitionProgress == 1;
            if (_selectedItem != null)
            {
                _connectionLine.SetPosition(0, _connectionLine.transform.TransformPoint(Vector3.zero));
                _connectionLine.SetPosition(1, _selectedItem.GetPosition());
            }

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


        private void TriggerClickedHandler(object sender, ClickedEventArgs clickedEventArgs)
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

            //var distanceToCurrentPos = Vector3.Distance(PositionFromController, transform.position);
            //var toggle = (distanceToCurrentPos < CLOSE_MENU_THRESHOLD_DISTANCE);
            switch (_state)
            {
                case States.Undefined:
                case States.Closing:
                case States.Closed:
                    SetState(States.Opening);
                    break;
            }
        }

        private const float CLOSE_MENU_THRESHOLD_DISTANCE = 0.3f;

        private void TriggerUnclickedHandler(object sender, ClickedEventArgs clickedEventArgs)
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

            //Debug.Log("set new state:" + _state + " -> " + newState);

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

        private const float TRANSITION_DURATION = 0.15f;
        private float _interactionStartTime = 0;

        public static InformationPanel _instance;
    }
}
