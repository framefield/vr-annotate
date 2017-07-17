using System;
using System.Collections;
using System.Collections.Generic;
using ff.nodegraph;
using ff.nodegraph.interaction;
using ff.vr.annotate;
using UnityEngine;


namespace ff.vr.interaction
{
    public class InfoPanelContentForAnnotations : MonoBehaviour, IInfoPanelContent
    {
        [Header("--- internal prefab references -----")]
        [SerializeField]
        SceneGraphPanel _sceneGraphPanel;

        public Vector3 GetConnectionLineStart()
        {
            return _sceneGraphPanel.PositionOfSelectedItem;
        }

        public void SetSelection(ISelectable newSelection)
        {
            var annotationGizmo = newSelection as AnnotationGizmo;
            var nodeOrNull = annotationGizmo != null ? annotationGizmo.Annotation.TargetNode : null;
            _sceneGraphPanel.SetSelectedNode(nodeOrNull);
        }
    }
}
