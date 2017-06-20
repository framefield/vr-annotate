using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ff.vr.interaction
{
    public class InformationPanel : MonoBehaviour
    {
        void Start()
        {
            foreach (SteamVR_TrackedController controller in Resources.FindObjectsOfTypeAll(typeof(SteamVR_TrackedController)))
            {
                controller.MenuButtonClicked += TriggerClickedHandler;
                controller.MenuButtonUnclicked += TriggerUnclickedHandler;
            }
        }

        void Update()
        {
            var transitionComplete = TransitionProgress == 1;

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

            var distanceToCurrentPos = Vector3.Distance(PositionFromController, transform.position);
            var toggle = (distanceToCurrentPos < CLOSE_MENU_THRESHOLD_DISTANCE);
            switch (_state)
            {
                case States.Undefined:
                case States.Closing:
                case States.Closed:
                    SetState(States.Opening);
                    break;

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
            _pressedMenuButtonController = null;
        }



        private void SetState(States newState)
        {
            if (newState == _state)
                return;

            Debug.Log("set new state:" + _state + " -> " + newState);

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
                    break;

                case States.Opening:
                    startNewTransition = true;
                    break;

                case States.Open:
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
            get { return _pressedMenuButtonController.transform.position + _pressedMenuButtonController.transform.forward; }
        }

        private Quaternion RotationFromController
        {
            get { return _pressedMenuButtonController.transform.rotation; }
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

        private const float TRANSITION_DURATION = 0.2f;
        private float _interactionStartTime = 0;
    }
}
