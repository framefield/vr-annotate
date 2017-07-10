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

        [SerializeField] Color HighlightColor = Color.blue;

        private Color _defaultColor;
        private Renderer _renderer;

        void Awake()
        {
            _renderer = GetComponent<Renderer>();
            _defaultColor = _renderer.material.GetColor("_tintColor");
        }

        void Update()
        {

        }

        private bool hitByLaser = false;

        #region Clickable 
        public void PointerEnter(LaserPointer pointer)
        {
            hitByLaser = true;
            _renderer.material.SetColor("_tintColor", HighlightColor);
            OnHover.Invoke();
        }

        public void PointerExit(LaserPointer pointer)
        {
            hitByLaser = false;
            _renderer.material.SetColor("_tintColor", _defaultColor);
            OnUnhover.Invoke();
        }

        public void PointerTriggered(LaserPointer pointer)
        {

        }

        public void PointerUntriggered(LaserPointer pointer)
        {

            if (hitByLaser)
            {
                OnClick.Invoke();
            }
        }

        public void PointerUpdate(LaserPointer pointer)
        {

        }
        #endregion Clickable
    }
}
