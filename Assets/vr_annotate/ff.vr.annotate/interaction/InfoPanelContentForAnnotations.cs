using System;
using System.Collections;
using System.Collections.Generic;
using ff.nodegraph;
using ff.nodegraph.interaction;
using ff.vr.annotate;
using ff.vr.annotate.datamodel;
using UnityEngine;


namespace ff.vr.interaction
{
    public class InfoPanelContentForAnnotations : MonoBehaviour, IInfoPanelContent
    {
        [Header("--- internal prefab references -----")]

        [SerializeField]
        TMPro.TextMeshPro _annotationObjectLabel;
        [SerializeField]
        TMPro.TextMeshPro _annotationBodyLabel;
        [SerializeField]
        TMPro.TextMeshPro _authorLabel;
        [SerializeField]
        TMPro.TextMeshPro _annotationDateLabel;

        public void ForwardSelectionFromInfoPanel(ISelectable newSelection)
        {
            var annotationGizmo = newSelection as AnnotationGizmo;
            _annotation = annotationGizmo != null ? annotationGizmo.Annotation : null;
            _targetNode = _annotation != null ? _annotation.TargetNode : null;
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
