using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ff.vr.interaction
{
    /* Switch to virtual keyboard mode if one of the controllers 
    comes too close to to PunchKeyboard.
    */
    public class KeyboardEnabler : MonoBehaviour
    {
        public bool IsVisible = false;
        public float ThresholdDistance = 0.6f;

        public InteractiveController[] _controllers;
        public KeyboardControllerStick[] _sticks;





        [Header("--- internal prefab references -----")]
        public GameObject PunchKeyboardObject;
        public InputField _inputField;

        public delegate void OnInputCompleted();
        public OnInputCompleted InputCompleted;

        public delegate void OnInputChanged(string newText);
        public OnInputChanged InputChanged;


        void Start()
        {
            UpdateKeyboardVisibility(forceUpdate: true);
        }

        public void Show()
        {
            PositionInFrontOfCamera();
            EventSystem.current.SetSelectedGameObject(_inputField.gameObject, null);
            IsVisible = true;
        }


        public void Hide()
        {
            IsVisible = false;
        }

        void Update()
        {
            if (Input.GetKeyUp(KeyCode.Return))
            {
                if (InputCompleted != null)
                {
                    InputCompleted();
                }
            }
            else if (Input.anyKey)
            {
                if (InputChanged != null)
                {
                    InputChanged(_inputField.text);
                }
            }
            UpdateKeyboardVisibility();
        }

        private void UpdateKeyboardVisibility(bool forceUpdate = false)
        {
            if (IsVisible != _wasEnabled || forceUpdate)
            {
                _wasEnabled = IsVisible;
                PunchKeyboardObject.SetActive(IsVisible);

                foreach (var stick in _sticks)
                {
                    stick.gameObject.SetActive(IsVisible);
                }

                if (IsVisible)
                {
                    // Focus KeyInput
                    EventSystem.current.SetSelectedGameObject(_inputField.gameObject, null);
                }
                else
                {

                }
            }
        }

        private void PositionInFrontOfCamera()
        {
            var forward = Camera.main.transform.forward * 0.5f;
            forward.y = 0;
            var pos = Camera.main.transform.position + forward + Vector3.down * 0.5f;

            this.transform.position = pos;
            var ea = Camera.main.transform.eulerAngles;
            ea.x = 0;
            ea.z = 0;
            transform.eulerAngles = ea;
        }

        private bool _wasEnabled = false;
    }
}