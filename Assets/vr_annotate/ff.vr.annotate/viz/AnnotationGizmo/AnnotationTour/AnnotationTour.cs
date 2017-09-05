using System.Collections;
using System.Collections.Generic;
using ff.utils;
using ff.vr.annotate;
using ff.vr.annotate.datamodel;
using ff.vr.interaction;
using UnityEngine;

public class AnnotationTour : Singleton<AnnotationTour>
{

    [SerializeField] GameObject _annotationContainer;
    [SerializeField] AnnotationGizmo[] _annotationGizmos;
    public int ISelectedGizmo;
    public int NumberOfNeighboursToRender;



    void Start()
    {
        // SelectionManager.Instance.SelectedAnnotationGizmoChangedEvent += SelectedAnnotationGizmoChangedHandler;
        _lineRenderer = GetComponent<LineRenderer>();
    }

    // void OnDisable()
    // {
    //     SelectionManager.Instance.SelectedAnnotationGizmoChangedEvent -= SelectedAnnotationGizmoChangedHandler;
    // }

    public AnnotationGizmo GetNextGizmo(AnnotationGizmo gizmo)
    {
        var iGizmo = GetIndexOfGizmoInList(gizmo);
        return _annotationGizmos[(iGizmo + 1) % _annotationGizmos.Length];
    }

    public AnnotationGizmo GetPreviousGizmo(AnnotationGizmo gizmo)
    {
        var iGizmo = GetIndexOfGizmoInList(gizmo);
        return _annotationGizmos[(iGizmo - 1 + _annotationGizmos.Length) % _annotationGizmos.Length];
    }


    // private void SelectedAnnotationGizmoChangedHandler(AnnotationGizmo annotationGizmo)
    // {
    //     if (_annotationGizmos == null || _annotationGizmos.Length < 1)
    //         _annotationGizmos = _annotationContainer.GetComponentsInChildren<AnnotationGizmo>();

    //     ISelectedGizmo = GetIndexOfGizmoInList(annotationGizmo);
    //     if (ISelectedGizmo > 0)
    //         ShowNeighborPhantoms(annotationGizmo);
    //     else
    //     {
    //         HideAllGizmosPhantoms();
    //     }
    // }

    // public void ShowNeighborPhantoms(AnnotationGizmo activeGizmo)
    // {
    //     HideAllGizmosPhantoms();
    //     ISelectedGizmo = GetIndexOfGizmoInList(activeGizmo);
    //     var numberOfNeighboursToRender = Mathf.Min(NumberOfNeighboursToRender, Mathf.FloorToInt(_annotationGizmos.Length - 1) / 2);

    //     _lineRenderer.enabled = true;
    //     _lineRenderer.positionCount = 2 * numberOfNeighboursToRender + 1;

    //     var pos = _annotationGizmos[ISelectedGizmo].Annotation.ViewPointPosition.position;
    //     pos = ProjectPosition(pos);
    //     _lineRenderer.SetPosition(numberOfNeighboursToRender, pos);

    //     for (int i = 1; i <= numberOfNeighboursToRender; i++)
    //     {
    //         float visibilityFactor = 1f - 1f * i / (numberOfNeighboursToRender + 1);

    //         var iRightNeighbour = (ISelectedGizmo + i) % _annotationGizmos.Length;
    //         var RightGizmo = _annotationGizmos[iRightNeighbour];
    //         RightGizmo.GetComponentInChildren<AnnotationGizmoButton>().AnnotationPositionRenderer.SetPhantom(visibilityFactor);
    //         _lineRenderer.SetPosition(numberOfNeighboursToRender + i, ProjectPosition(RightGizmo.Annotation.ViewPointPosition.position));

    //         var iLeftNeighbour = (ISelectedGizmo - i) % _annotationGizmos.Length;
    //         var LeftGizmo = _annotationGizmos[iLeftNeighbour];
    //         LeftGizmo.GetComponentInChildren<AnnotationGizmoButton>().AnnotationPositionRenderer.SetPhantom(visibilityFactor);
    //         _lineRenderer.SetPosition(numberOfNeighboursToRender - i, ProjectPosition(LeftGizmo.Annotation.ViewPointPosition.position));
    //     }
    // }

    // public void HideAllGizmosPhantoms()
    // {
    //     foreach (var gizmo in _annotationGizmos)
    //     {
    //         gizmo.GetComponentInChildren<AnnotationGizmoButton>().AnnotationPositionRenderer.SetPhantom(0.0f);
    //     }
    //     _lineRenderer.enabled = false;
    // }

    private int GetIndexOfGizmoInList(AnnotationGizmo gizmo)
    {
        if (_annotationGizmos == null || _annotationGizmos.Length < 1)
            _annotationGizmos = _annotationContainer.GetComponentsInChildren<AnnotationGizmo>();

        for (int iGizmo = 0; iGizmo < _annotationGizmos.Length; iGizmo++)
        {
            if (_annotationGizmos[iGizmo] == gizmo)
                return iGizmo;
        }
        return -1;
    }

    public float PROJECTIONHEIGHT;
    private Vector3 ProjectPosition(Vector3 position)
    {
        return new Vector3(position.x, PROJECTIONHEIGHT, position.z);
    }

    private LineRenderer _lineRenderer;
}


