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
}


