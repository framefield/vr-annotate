using System;
using System.Collections;
using System.Collections.Generic;
using ff.vr.interaction;
using UnityEngine;

namespace ff.nodegraph.interaction
{
    public class NodeStateRenderer : MonoBehaviour
    {

        [SerializeField] Color DefaultColor;
        [SerializeField] Color HovererdColor;
        [SerializeField] Color SelectedColor;
        [SerializeField] Color HovererdSelectedColor;

        void Start()
        {
            _allRenderer = GetComponentsInChildren<Renderer>();
            _allTextLabels = GetComponentsInChildren<TMPro.TextMeshPro>();

            SelectionManager.Instance.OnSelectedNodeChanged += OnSelectedNodeChangedHandler;
            SelectionManager.Instance.OnNodeHover += OnNodeHoverHandler;
            SelectionManager.Instance.OnNodeUnhover += OnNodeUnhoverHandler;
        }

        private void OnNodeHoverHandler(ISelectable obj)
        {
            if (obj != _renderedNode)
                return;

            var isSelected = SelectionManager.Instance.SelectedNode == _renderedNode;
            var color = isSelected ? HovererdSelectedColor : HovererdColor;
            SetColor(color);
        }

        private void OnNodeUnhoverHandler(ISelectable obj)
        {
            if (obj != _renderedNode)
                return;

            var isSelected = SelectionManager.Instance.SelectedNode == _renderedNode;
            var color = isSelected ? SelectedColor : DefaultColor;
            SetColor(color);
        }

        private void OnSelectedNodeChangedHandler(Node obj)
        {
            if (obj != _renderedNode)
                return;
            SetColor(HovererdSelectedColor);
        }

        public void SetNodeToRender(Node node)
        {
            _renderedNode = node;
        }

        private void SetColor(Color color)
        {
            foreach (var r in _allRenderer)
                r.material.color = color;

            foreach (var txtl in _allTextLabels)
                txtl.color = color;
        }

        private Renderer[] _allRenderer;
        private TMPro.TextMeshPro[] _allTextLabels;

        private Node _renderedNode;
    }
}
