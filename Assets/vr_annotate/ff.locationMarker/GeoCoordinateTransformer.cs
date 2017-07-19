using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ff.location
{
    /** Provides a service to convert between unity-worldspace coordinates and WGS85. */
    public class GeoCoordinateTransformer : MonoBehaviour
    {
        void Awake()
        {
            _locationMarkers = FindObjectsOfType<GeoCoordinateMarker>();
            if (_locationMarkers.Length != 2)
            {
                Debug.LogWarning("Correct geospacial mapping from WGS to Unity requires exactly two LocationMarkers");
                return;
            }

            var A = _locationMarkers[0];
            var B = _locationMarkers[1];


            _referenceLat = A.latitude;
            _referenceLong = A.longitude;
            InitializeMetersPerLat((float)A.latitude);

            var vInUnity = A.transform.position - B.transform.position;
            var rotationInUnity = Mathf.Atan2(vInUnity.x, vInUnity.z) * 180 / Mathf.PI;

            var vInWGS = new Vector2((float)((A.longitude - B.longitude) * _metersPerLong),
                                          (float)((A.latitude - B.latitude) * _metersPerLat));
            var rotationInWGS = Mathf.Atan2(vInWGS.x, vInWGS.y) * 180 / Mathf.PI;

            // Debug.Log("_metersPerLat:" + _metersPerLat);
            // Debug.Log("_metersPerLong:" + _metersPerLong);
            // Debug.Log("rotationWGS:" + rotationInWGS);
            // Debug.Log("rotationY:" + rotationInUnity);
            var factor = 1 - Mathf.Abs(1 - vInUnity.magnitude / vInWGS.magnitude);
            Debug.LogFormat("GeoCoordinateTransform precision: {0:0}%", factor * 100, this);

            this.transform.position = A.transform.position;
            this.transform.Rotate(Vector3.up, rotationInUnity - rotationInWGS);
            T = this.transform;
        }

        internal static double _metersPerLat;
        internal static double _metersPerLong;
        internal static double _referenceLat;
        internal static double _referenceLong;
        internal static Transform T;

        //This function is a modified version of the JavaScript found at http://www.csgnetwork.com/degreelenllavcalc.html (C) CSGNetwork.COM and Computer Support Group	
        private void InitializeMetersPerLat(float lat) // Compute lengths of degrees
        {
            // Set up "Constants"
            double m1 = 111132.92;      // latitude calculation term 1
            double m2 = -559.82;        // latitude calculation term 2
            double m3 = 1.175;          // latitude calculation term 3
            double m4 = -0.0023;        // latitude calculation term 4
            double p1 = 111412.84;      // longitude calculation term 1
            double p2 = -93.5;          // longitude calculation term 2
            double p3 = 0.118;          // longitude calculation term 3

            // Convert latitude to radians
            lat = (float)((lat * Mathf.PI) / 180.0);

            // Calculate the length of a degree of latitude and longitude in meters
            _metersPerLat = m1 + (m2 * Mathf.Cos(2 * lat)) + (m3 * Mathf.Cos(4 * lat)) + (m4 * Mathf.Cos(6 * lat));
            _metersPerLong = (p1 * Mathf.Cos(lat)) + (p2 * Mathf.Cos(3 * lat)) + (p3 * Mathf.Cos(5 * lat));
        }

        private GeoCoordinateMarker[] _locationMarkers;
    }
}
