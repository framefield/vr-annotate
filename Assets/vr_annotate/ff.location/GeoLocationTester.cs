using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ff.location
{
    public class GeoLocationTester : MonoBehaviour
    {
        public GeoCoordinate _geoCoordinate;
        public string _latLong;

        void Update()
        {
            _geoCoordinate.position = this.transform.position;
            _geoCoordinate.CalcLatLongFromPosition();
            _latLong = string.Format("{0}, {1}", _geoCoordinate.latitude, _geoCoordinate.longitude);
        }
    }
}
