using System;
using System.Collections;
using System.Collections.Generic;
using ff.nodegraph;
using ff.utils;
using ff.vr.interaction;
using UnityEngine;

namespace ff.nodegraph.interaction
{
    public class NodeSelectionRenderer : MonoBehaviour
    {
        [Header("--- prefab references ----")]

        [SerializeField]
        TMPro.TextMeshPro _hoverLabel;

        [SerializeField]
        Renderer _highlightSelectionRenderer;

        [SerializeField]
        Renderer _highlightHoverRenderer;

        // Use this for initialization
        void Start()
        {
            SelectionManager.Instance.OnSelectedNodeChanged += NodeSelectionChangedHander;
            SelectionManager.Instance.OnNodeHover += OnNodeHoverHandler;
            SelectionManager.Instance.OnNodeUnhover += OnNodeUnhoverHandler;

        }

        private void OnNodeHoverHandler(ISelectable obj)
        {
            if (!(obj is Node))
                return;
            _hoverLabel.gameObject.SetActive(true);
            _highlightHoverRenderer.enabled = true;
            var hoveredNode = obj as Node;
            _hoverLabel.text = hoveredNode.Name;
            var boundsWithContext = hoveredNode.CollectBoundsWithContext().ToArray();
            _highlightHoverRenderer.GetComponent<MeshFilter>().mesh = GenerateMeshFromBounds.GenerateMesh(boundsWithContext);
        }

        private void OnNodeUnhoverHandler(ISelectable obj)
        {
            if (!(obj is Node))
                return;
            _hoverLabel.gameObject.SetActive(false);
            _highlightHoverRenderer.enabled = false;
        }


        private void NodeSelectionChangedHander(Node selectedNodeOrNull)
        {
            if (selectedNodeOrNull == null)
            {
                _highlightSelectionRenderer.enabled = false;
                return;
            }

            var boundsWithContext = selectedNodeOrNull.CollectBoundsWithContext().ToArray();
            _highlightSelectionRenderer.GetComponent<MeshFilter>().mesh = GenerateMeshFromBounds.GenerateMesh(boundsWithContext);
            _highlightSelectionRenderer.enabled = true;
        }
    }
}