using UnityEngine;


namespace ff.vr.interaction
{

    public abstract class LaserPointerStyle : ILaserPointerAppearance
    {
        public Color LaserColor
        {
            get
            {
                return _laserColor;
            }
        }


        public GameObject HitIndicator
        {
            get
            {
                return _hitIndicator;
            }
        }


        protected Color _laserColor;
        protected Color _highlightColor;
        protected GameObject _hitIndicator;
    }

    public class MoveRotateLaserPointerStyle : LaserPointerStyle
    {
        public MoveRotateLaserPointerStyle()
        {
            var blue = new Color(69f / 255f, 146f / 255f, 255f / 255f);
            _laserColor = blue;
            _highlightColor = blue;
        }
    }

    public class DeleteLaserPointerStyle : LaserPointerStyle
    {
        public DeleteLaserPointerStyle()
        {
            var red = new Color(255f / 255f, 64f / 255f, 158f / 255f);
            _laserColor = red;
            _highlightColor = red;
        }
    }

    public class DuplicateLaserPointerStyle : LaserPointerStyle
    {
        public DuplicateLaserPointerStyle()
        {
            var blue = new Color(69f / 255f, 146f / 255f, 255f / 255f);
            _laserColor = blue;
            _highlightColor = blue;
        }
    }

    public class SwitchVariationLaserPointerStyle : LaserPointerStyle
    {
        public SwitchVariationLaserPointerStyle()
        {
            var blue = new Color(69f / 255f, 146f / 255f, 255f / 255f);
            _laserColor = blue;
            _highlightColor = blue;
        }
    }

    public class TeleportationLaserPointerStyle : LaserPointerStyle
    {
        public TeleportationLaserPointerStyle()
        {
            var green = new Color(27f / 255f, 204f / 255f, 109f / 255f);
            _laserColor = green;
            _highlightColor = green;
        }
    }

    public class InactiveLaserPointerStyle : LaserPointerStyle
    {
        public InactiveLaserPointerStyle()
        {
            _laserColor = Color.gray;
            _highlightColor = Color.gray;
        }
    }
}