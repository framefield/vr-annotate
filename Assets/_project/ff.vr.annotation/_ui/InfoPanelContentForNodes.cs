using System;
using System.Collections;
using System.Collections.Generic;
using ff.vr.interaction;
using UnityEngine;

namespace ff.nodegraph.interaction
{
    public class InfoPanelContentForNodes : MonoBehaviour, IInfoPanelContent
    {
        [Header("--- internal prefab references ----")]
        [SerializeField]
        SceneGraphPanel _sceneGraphPanel;

        public void SetSelection(ISelectable newSelection)
        {
        }

        public void SetSelectedNode(Node newNode)
        {
            _selectedNode = newNode;
            _sceneGraphPanel.SetSelectedNode(_selectedNode);
        }

        private InfoPanel _infoPanel;
        private Node _selectedNode;
    }
}
