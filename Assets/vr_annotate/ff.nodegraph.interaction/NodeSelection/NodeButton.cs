using System.Collections;
using System.Collections.Generic;
using ff.vr.interaction;
using UnityEngine;

namespace ff.nodegraph.interaction
{
    public class NodeButton : MonoBehaviour, IClickableLaserPointerTarget
    {
        public void SetNode(Node node)
        {
            _node = node;
        }

        void IClickableLaserPointerTarget.PointerTriggered(LaserPointer pointer)
        {
        }

        void IClickableLaserPointerTarget.PointerUntriggered(LaserPointer pointer)
        {
            SelectionManager.Instance.SetSelectedItem(_node);
        }

        void ILaserPointerTarget.PointerEnter(LaserPointer pointer)
        {
            SelectionManager.Instance.SetOnNodeHover(_node);
        }

        void ILaserPointerTarget.PointerExit(LaserPointer pointer)
        {
            SelectionManager.Instance.SetOnNodeUnhover(_node);
        }

        void ILaserPointerTarget.PointerUpdate(LaserPointer pointer)
        {
        }

        private Node _node;
    }
}