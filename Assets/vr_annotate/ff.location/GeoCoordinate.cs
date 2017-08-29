using UnityEngine;

namespace ff.location
{

    [System.Serializable]
    public class GeoCoordinate : ISerializationCallbackReceiver
    {
        public Vector3 position;
        public Vector3 rotation;

        [SerializeField] string type = "GeoCoordinates";
        [SerializeField] string coordinateSystem = "Unity.WorldSpace";
        [SerializeField] internal double latitude;
        [SerializeField] internal double longitude;
        [SerializeField] internal double elevation;

        public void OnAfterDeserialize()
        {

        }

        public void OnBeforeSerialize()
        {
            CalcLatLongFromPosition();
        }

        internal void CalcLatLongFromPosition()
        {
            if (GeoCoordinateTransformer.T == null)
                return;

            var inRotatedObjectSpace = GeoCoordinateTransformer.T.InverseTransformPoint(position);
            longitude = inRotatedObjectSpace.x / GeoCoordinateTransformer._metersPerLong + GeoCoordinateTransformer._referenceLong;
            latitude = inRotatedObjectSpace.z / GeoCoordinateTransformer._metersPerLat + GeoCoordinateTransformer._referenceLat;
            elevation = inRotatedObjectSpace.y / (GeoCoordinateTransformer._metersPerLat + GeoCoordinateTransformer._metersPerLong) * 2;
        }

        // These values will be serialized but not directly used
        private void DummyToAvoidNeverUsedWarning()
        {
            Debug.Log("" + type);
            Debug.Log("" + coordinateSystem);
        }
    }
}
