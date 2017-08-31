using System;
using System.Collections;
using System.Collections.Generic;
using ff.vr.interaction;
using UnityEngine;


namespace ff.nodegraph.interaction
{
    public class NodeGraphOutlinerItem : MonoBehaviour
    {
        [Header("Label Color")]
        public Color LabelColor;
        public Color HoveredLabelColor;
        public Color SelectedLabelColor;

        [Header("Background Color")]
        public Color BackgroundColor;
        public Color HoveredBackgorundColor;
        public Color SelectedBackgroundColor;

        public bool IsSelected;
        private bool _isHovered;

        [Header("--- internal prefab references ----")]
        [SerializeField]
        LaserPointerButton _button;

        [SerializeField]
        TMPro.TextMeshPro _label;

        public NodeGraphOutliner SceneGraphPanel { get; set; }

        public void OnEnable()
        {
            SelectionManager.Instance.OnHover += OnHoverHandler;
            SelectionManager.Instance.OnUnhover += OnUnhoverHandler;
        }

        public void OnDisable()
        {
            SelectionManager.Instance.OnHover -= OnHoverHandler;
            SelectionManager.Instance.OnUnhover -= OnUnhoverHandler;
        }

        private void OnHoverHandler(ISelectable obj)
        {
            if (_node == obj as Node)
            {
                _isHovered = true;
                UpdateUI();
            }
        }

        private void OnUnhoverHandler(ISelectable obj)
        {

            if (_node == obj as Node)
            {
                _isHovered = false;
                UpdateUI();

            }
        }

        public Node Node
        {
            get { return _node; }
            set
            {
                _node = value;
                UpdateUI();
            }
        }

        public string Text
        {
            get { return _label.text; }
            set { _label.text = value; }
        }

        public int Indentation
        {
            set { _label.transform.localPosition = Vector3.right * value * INDENTATION_WIDHT; }
        }

        private void UpdateUI()
        {
            Color backgroundColor;
            Color labelColor;

            if (IsSelected)
            {
                labelColor = SelectedLabelColor;
                backgroundColor = SelectedBackgroundColor;
            }
            else if (_isHovered)
            {
                labelColor = HoveredLabelColor;
                backgroundColor = HoveredBackgorundColor;
            }
            else
            {
                labelColor = LabelColor;
                backgroundColor = BackgroundColor;
            }
            _button.SetColor(backgroundColor);
            _label.color = labelColor;
        }

        public void OnClicked()
        {
            SelectionManager.Instance.SelectItem(_node);
        }

        public void OnHover()
        {
            SelectionManager.Instance.SetOnHover(_node);
        }

        public void OnUnhover()
        {
            SelectionManager.Instance.SetOnUnhover(_node);
        }

        private float INDENTATION_WIDHT = 0.1f;
        private Node _node;
    }
}