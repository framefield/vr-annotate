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
        public bool IsKeyboardEnabled = false;
        public float ThresholdDistance = 0.6f;

        public InteractiveController[] _controllers;
        public KeyboardControllerStick[] _sticks;

        [Header("--- internal prefab references -----")]
        public GameObject PunchKeyboardObject;
        public InputField _inputField;


        void Start()
        {
            SetIsKeyboardEnabled(false, forceUpdate: true);
        }

        void Update()
        {
            if (_controllers == null)
                return;

            var tooClose = false;
            foreach (var controller in _controllers)
            {
                var distance = Vector3.Distance(controller.transform.position, this.transform.position);
                tooClose |= distance < ThresholdDistance;
            }
            SetIsKeyboardEnabled(tooClose);
        }

        private void SetIsKeyboardEnabled(bool newState, bool forceUpdate = true)
        {
            if (newState != IsKeyboardEnabled || forceUpdate)
            {
                IsKeyboardEnabled = newState;
                PunchKeyboardObject.SetActive(newState);

                foreach (var stick in _sticks)
                {
                    stick.gameObject.SetActive(IsKeyboardEnabled);
                }

                if (IsKeyboardEnabled)
                {
                    EventSystem.current.SetSelectedGameObject(_inputField.gameObject, null);
                    //EventSystemManager.currentSystem.SetSelectedGameObject(_inputField.gameObject, null);
                    //_inputField.OnPointerClick(null);
                }
                else
                {

                }
            }
        }
    }
}