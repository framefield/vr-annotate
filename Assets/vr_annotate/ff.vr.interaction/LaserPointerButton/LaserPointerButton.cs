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

        #region Clickable 
        public virtual void PointerEnter(LaserPointer pointer)
        {
            _hitByLaser = true;
            UpdateUI();
            OnHover.Invoke();
        }

        public virtual void PointerExit(LaserPointer pointer)
        {
            _hitByLaser = false;
            UpdateUI();
            OnUnhover.Invoke();
        }

        public virtual void PointerTriggered(LaserPointer pointer) { }

        public virtual void PointerUntriggered(LaserPointer pointer)
        {
            if (_hitByLaser)
            {
                OnClick.Invoke();
            }
        }

        public virtual void PointerUpdate(LaserPointer pointer) { }


        public void SetColor(Color newColor)
        {
            Color = newColor;
            UpdateUI();
        }


        private void UpdateUI()
        {
            if (_renderer == null)
                _renderer = GetComponent<Renderer>();

            _renderer.material.SetColor("_tintColor", _hitByLaser ? HoverColor : Color);
        }

        #endregion Clickable

        private bool _hitByLaser = false;
        private Renderer _renderer;
        private bool _isSelected;
    }
}
