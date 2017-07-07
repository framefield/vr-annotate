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
        [SerializeField] Color HighlightColor;

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
        }

        public void PointerExit(LaserPointer pointer)
        {
            hitByLaser = false;
            _renderer.material.SetColor("_tintColor", _defaultColor);
        }

        public void PointerTriggered(LaserPointer pointer)
        {

            //throw new NotImplementedException();
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
            //throw new NotImplementedException();
        }
        #endregion Clickable
    }
}
