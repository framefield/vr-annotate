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

        [SerializeField]
        TMPro.TextMeshPro _annotationObjectLabel;
        [SerializeField]
        TMPro.TextMeshPro _annotationBodyLabel;
        [SerializeField]
        TMPro.TextMeshPro _authorLabel;
        [SerializeField]
        TMPro.TextMeshPro _annotationDateLabel;

        public Vector3 GetConnectionLineStart()
        {
            return _sceneGraphPanel.PositionOfSelectedItem;
        }

        public void SetSelection(ISelectable newSelection)
        {
            var annotationGizmo = newSelection as AnnotationGizmo;
            _annotation = annotationGizmo != null ? annotationGizmo.Annotation : null;
            _targetNode = _annotation != null ? _annotation.TargetNode : null;
            _sceneGraphPanel.SetSelectedNode(_targetNode);

            UpdateUI();
        }

        private void UpdateUI()
        {
            _annotationObjectLabel.text = _targetNode != null ? _targetNode.Name : "<Without Object>"; // FIXME: Needs to be implemented
            _annotationBodyLabel.text = _annotation != null ? _annotation.Text : "";
            _authorLabel.text = _annotation != null ? _annotation.Author.name : "";
            _annotationDateLabel.text = _annotation != null ? _annotation.CreatedAt.ToString("yyyy/MM/dd") : "";
        }

        private Annotation _annotation;
        private Node _targetNode;
    }
}
