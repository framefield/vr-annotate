using System;
using System.Collections;
using System.Collections.Generic;
using ff.vr.annotate.viz;
using ff.vr.interaction;
using UnityEngine;


namespace ff.nodegraph.interaction
{
    public class NodeGraphInfoPanelItem : MonoBehaviour, IClickableLaserPointerTarget
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
        TMPro.TextMeshPro _label;
        [SerializeField]
        TMPro.TextMeshPro _annotationCount;

        public NodeGraphInfoPanel SceneGraphPanel { get; set; }

        public void OnEnable()
        {
            SelectionManager.Instance.OnNodeHover += OnHoverHandler;
            SelectionManager.Instance.OnNodeUnhover += OnUnhoverHandler;
        }

        public void OnDisable()
        {
            SelectionManager.Instance.OnNodeHover -= OnHoverHandler;
            SelectionManager.Instance.OnNodeUnhover -= OnUnhoverHandler;
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


        public void InitNumberOfAnnotations()
        {
            _annotationCount.text = "" + GetNumberOfAnnotations();
        }

        public int Indentation
        {
            set { _label.transform.localPosition = Vector3.right * value * INDENTATION_WIDHT; }
        }

        public int GetNumberOfAnnotations()
        {
            var nodesAnnotations = AnnotationManager.Instance.GetAllAnnotationsGizmosOnNode(_node);
            return nodesAnnotations.Count;
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
            var renderer = GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.SetColor("_tintColor", backgroundColor);
                renderer.material.SetFloat("_tintAmount", 1f);
            }
            _label.color = labelColor;
            _annotationCount.color = labelColor;
        }


        public void PointerTriggered(LaserPointer pointer)
        {
            SelectionManager.Instance.SetSelectedItem(_node);
        }

        public void PointerUntriggered(LaserPointer pointer)
        {
        }

        public void PointerEnter(LaserPointer pointer)
        {
            SelectionManager.Instance.SetOnNodeHover(_node);
        }

        public void PointerExit(LaserPointer pointer)
        {
            SelectionManager.Instance.SetOnNodeUnhover(_node);
        }

        public void PointerUpdate(LaserPointer pointer)
        {
        }

        private float INDENTATION_WIDHT = 0.03f;
        private Node _node;
    }
}