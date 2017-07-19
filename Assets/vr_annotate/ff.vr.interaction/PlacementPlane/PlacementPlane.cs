using UnityEngine;

namespace ff.vr.interaction
{
    public class PlacementPlane : MonoBehaviour, IHoverColor, ILaserPointerTarget
    {
        public Color HoverColor;

        public Color GetHoverColor()
        {
            return HoverColor;
        }

        public void PointerEnter(LaserPointer pointer)
        {
        }

        public void PointerExit(LaserPointer pointer)
        {
        }

        public void PointerUpdate(LaserPointer pointer)
        {
        }
    }
}