using UnityEngine;

namespace ff.vr.interaction
{
    public class Teleportation : MonoBehaviour
    {

        public ReflectionProbe[] UserReflectionProbes;

        void Start()
        {
            Transform eyeCamera = FindObjectOfType<SteamVR_Camera>().GetComponent<Transform>();
            _HmdReference = eyeCamera.parent;
            _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        }


        void Update()
        {
            if (_isTeleporting)
            {
                var progress = (Time.time - _teleportationStartTime) / TELEPORTATION_DURATION;
                if (progress > 1)
                {
                    _isTeleporting = false;
                    progress = 1;
                }
                var smoothed = Mathf.SmoothStep(0, 1, progress);
                _HmdReference.position = Vector3.Lerp(_teleportationStartPosition, _teleportationEndPosition, smoothed);
                _HmdReference.rotation = Quaternion.Slerp(_teleportationStartRotation, _teleportationEndRotation, smoothed);
            }
        }

        //FIXME: This should use local offset and rotation
        public void JumpToSpot(TeleportationSpot spot)
        {
            if (_isTeleporting || spot == null)
                return;

            _teleportationStartPosition = _HmdReference.position;
            _teleportationStartRotation = _HmdReference.rotation;
            _teleportationEndPosition = spot.transform.position + GetHorizontalCamDistanceFromReference();
            _teleportationEndRotation = spot.transform.rotation;

            _teleportationStartTime = Time.time;
            _isTeleporting = true;

        }


        //FIXME: Does should use local offset and rotation
        public void JumpToPosition(Vector3 position)
        {
            if (_isTeleporting)
                return;

            _teleportationStartPosition = _HmdReference.position;
            _teleportationStartRotation = _HmdReference.rotation;
            _teleportationEndPosition = position + GetHorizontalCamDistanceFromReference();
            _teleportationEndRotation = _HmdReference.rotation;
            _teleportationStartTime = Time.time;
            _isTeleporting = true;

        }


        // Note: This doesn't work with rotated HMD orientation
        Vector3 GetHorizontalCamDistanceFromReference()
        {
            var deltaCamToRef = _HmdReference.position - _mainCamera.transform.position;
            var horziontalDelta = new Vector3(deltaCamToRef.x, 0, deltaCamToRef.z);
            return horziontalDelta;
        }

        Transform _HmdReference;
        private GameObject _mainCamera;

        private Vector3 _teleportationStartPosition;
        private Quaternion _teleportationStartRotation;
        private Vector3 _teleportationEndPosition;
        private Quaternion _teleportationEndRotation;
        private float _teleportationStartTime;
        private bool _isTeleporting;
        private const float TELEPORTATION_DURATION = 0.1f;
    }
}