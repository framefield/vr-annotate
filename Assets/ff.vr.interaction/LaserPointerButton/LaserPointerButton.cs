using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ff.vr.interaction
{
    public class LaserPointerButton : MonoBehaviour, IClickableLaserPointerTarget
    {
        public UnityEvent OnClick;
        public UnityEvent OnHover;
        public UnityEvent OnUnhover;

        public Color HoverColor = Color.blue;

        public Color Color = Color.gray;

        public bool IsSelected
        {
            get; set;
        }

        void Awake()
        {
            _renderer = GetComponent<Renderer>();
            //Color = _renderer.material.GetColor("_tintColor");
        }


        #region Clickable 
        public void PointerEnter(LaserPointer pointer)
        {
            _hitByLaser = true;
            UpdateUI();
            OnHover.Invoke();
        }

        public void PointerExit(LaserPointer pointer)
        {
            _hitByLaser = false;
            UpdateUI();
            OnUnhover.Invoke();
        }

        public void PointerTriggered(LaserPointer pointer) { }

        public void PointerUntriggered(LaserPointer pointer)
        {

            if (_hitByLaser)
            {
                OnClick.Invoke();
            }
        }

        public void PointerUpdate(LaserPointer pointer)
        {

        }

        public void UpdateUI()
        {
            _renderer.material.SetColor("_tintColor", _hitByLaser ? HoverColor : Color);
        }

        #endregion Clickable

        private bool _hitByLaser = false;
        private Renderer _renderer;
        private bool _isSelected;
    }
}
