using System;
using System.Collections;
using System.Collections.Generic;
using ff.nodegraph;
using ff.vr.annotate;
using ff.vr.interaction;
using UnityEngine;

public class HapticFeedback : MonoBehaviour
{
    SteamVR_TrackedObject trackedObj;
    SteamVR_Controller.Device device;
    // Use this for initialization
    void Start()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
        device = SteamVR_Controller.Input((int)trackedObj.index);
    }

    void OnEnable()
    {
        SelectionManager.Instance.SelectedAnnotationGizmoChangedEvent += SelectedAnnotationGizmoChangedHandler;
        SelectionManager.Instance.OnAnnotationGizmoHover += OnAnnotationGizmoHoverHandler;
        SelectionManager.Instance.OnAnnotationGizmoUnhover += OnAnnotationGizmoUnhoverHandler;
        SelectionManager.Instance.SelectedNodeChangedEvent += SelectedNodeChangedHandler;
        SelectionManager.Instance.OnNodeHover += OnNodeHoverHandler;
        SelectionManager.Instance.OnNodeUnhover += OnNodeUnhoverHandler;
    }

    private void OnNodeUnhoverHandler(ISelectable obj)
    {
        device.TriggerHapticPulse(150);
    }

    private void OnNodeHoverHandler(ISelectable obj)
    {
        device.TriggerHapticPulse(150);
    }

    private void SelectedNodeChangedHandler(Node obj)
    {
        device.TriggerHapticPulse(1000);
    }

    private void OnAnnotationGizmoUnhoverHandler(AnnotationGizmo obj)
    {
        device.TriggerHapticPulse(150);
    }

    void OnDisable()
    {
        SelectionManager.Instance.SelectedAnnotationGizmoChangedEvent -= SelectedAnnotationGizmoChangedHandler;
        SelectionManager.Instance.OnAnnotationGizmoHover -= OnAnnotationGizmoHoverHandler;
        SelectionManager.Instance.OnAnnotationGizmoUnhover -= OnAnnotationGizmoUnhoverHandler;
        SelectionManager.Instance.SelectedNodeChangedEvent -= SelectedNodeChangedHandler;
        SelectionManager.Instance.OnNodeHover -= OnNodeHoverHandler;
        SelectionManager.Instance.OnNodeUnhover -= OnNodeUnhoverHandler;
    }

    private void OnAnnotationGizmoHoverHandler(AnnotationGizmo obj)
    {
        device.TriggerHapticPulse(150);
    }

    private void SelectedAnnotationGizmoChangedHandler(AnnotationGizmo obj)
    {
        device.TriggerHapticPulse(1000);
    }

}
