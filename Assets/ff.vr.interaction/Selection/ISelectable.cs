using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ff.vr.interaction
{
    public interface ISelectable
    {
        void OnSelected();
        Vector3 GetPosition();
    }
}
