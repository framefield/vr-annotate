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
            _sceneGraphPanel.SetSelectedNode(newSelection as Node);
        }

        public Vector3 GetConnectionLineStart()
        {
            return _sceneGraphPanel.PositionOfSelectedItem;
        }

        private InfoPanel _infoPanel;
    }
}
