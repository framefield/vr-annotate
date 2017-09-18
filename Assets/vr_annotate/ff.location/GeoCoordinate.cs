using UnityEngine;

namespace ff.location
{

    [System.Serializable]
    public class GeoCoordinate : ISerializationCallbackReceiver
    {
        [SerializeField] string type = "GeoCoordinates";
        [SerializeField] string coordinateSystem = "Unity.WorldSpace";

        public Vector3 position;
        [SerializeField] internal double latitude;
        [SerializeField] internal double longitude;
        [SerializeField] internal double elevation;

        public Vector3 positionViewport;
        [SerializeField] internal double latitudeViewPort;
        [SerializeField] internal double longitudeViewPort;
        [SerializeField] internal double elevationViewPort;

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

            var viewPortInRotatedObjectSpace = GeoCoordinateTransformer.T.InverseTransformPoint(positionViewport);
            longitudeViewPort = viewPortInRotatedObjectSpace.x / GeoCoordinateTransformer._metersPerLong + GeoCoordinateTransformer._referenceLong;
            latitudeViewPort = viewPortInRotatedObjectSpace.z / GeoCoordinateTransformer._metersPerLat + GeoCoordinateTransformer._referenceLat;
            elevationViewPort = viewPortInRotatedObjectSpace.y / (GeoCoordinateTransformer._metersPerLat + GeoCoordinateTransformer._metersPerLong) * 2;
        }

        // These values will be serialized but not directly used
        private void DummyToAvoidNeverUsedWarning()
        {
            Debug.Log("" + type);
            Debug.Log("" + coordinateSystem);
        }
    }
}
