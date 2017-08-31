using System;
using System.Collections;
using System.Collections.Generic;
using ff.nodegraph;
using ff.utils;
using ff.vr.interaction;
using UnityEngine;

public class NodeSelectionRenderer : MonoBehaviour
{
    [Header("--- prefab references ----")]

    [SerializeField]
    TMPro.TextMeshPro _hoverLabel;

    [SerializeField]
    Renderer _highlightContextRenderer;

    [SerializeField]
    Renderer _highlightHoverRenderer;

    // Use this for initialization
    void Start()
    {
        SelectionManager.Instance.SelectedNodeChangedEvent += NodeSelectionChangedHander;
        SelectionManager.Instance.OnHover += OnHoverHandler;
        SelectionManager.Instance.OnUnhover += OnUnhoverHandler;

    }

    private void OnHoverHandler(ISelectable obj)
    {
        if (!(obj is Node))
            return;
        _hoverLabel.gameObject.SetActive(true);
        _highlightHoverRenderer.enabled = true;
        var HoveredNode = obj as Node;
        _hoverLabel.text = HoveredNode.Name;
        var allBoundsWithContext = HoveredNode.CollectBoundsWithContext().ToArray();

        _highlightHoverRenderer.GetComponent<MeshFilter>().mesh = GenerateMeshFromBounds.GenerateMesh(allBoundsWithContext);
        var boundsWithContext = HoveredNode.CollectBoundsWithContext().ToArray();

        _highlightContextRenderer.GetComponent<MeshFilter>().mesh = GenerateMeshFromBounds.GenerateMesh(boundsWithContext);

        _highlightContextRenderer.enabled = true;
    }

    private void OnUnhoverHandler(ISelectable obj)
    {
        if (!(obj is Node))
            return;
        _hoverLabel.gameObject.SetActive(false);
        _highlightHoverRenderer.enabled = false;
        _highlightContextRenderer.enabled = false;

    }


    private void NodeSelectionChangedHander(Node nodeOrNull)
    {


    }

}
