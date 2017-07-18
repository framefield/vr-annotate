using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ff.vr.interaction
{
    public interface ISelectable
    {
        //bool IsSelected {get;set;}
        Vector3 GetPosition();
    }
}
