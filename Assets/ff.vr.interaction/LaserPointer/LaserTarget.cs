using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;


namespace ff.vr.interaction
{

    /// <summary>
    /// Component that makes targets Clickable
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class LaserTarget : MonoBehaviour, ILaserPointerTarget, IClickableLaserPointerTarget
    {
        void Start()
        {
            _renderer = GetComponent<Renderer>();
            _audioSource = GetComponent<AudioSource>();
        }

        AudioSource _audioSource;

        [System.Serializable]
        public class TriggeredEvent : UnityEvent { }

        public TriggeredEvent OnTriggered;

        public void PointerEnter(LaserPointer pointer)
        {
            _originalEmissionColor = _renderer.material.GetColor("_EmissionColor");
            _renderer.material.SetColor("_EmissionColor", new Color(1, 1, 0.2f, 0.4f));
            if (_audioSource)
                _audioSource.Play();
        }

        public void PointerTriggered(LaserPointer pointer)
        {
            if (_state != State.Default)
                Debug.LogError("Capture Pointer if not in State.Default?");

            _state = State.PointerCaptured;
        }


        public void PointerUntriggered(LaserPointer pointer)
        {
            if (_state != State.PointerCaptured)
                Debug.LogError("Releasing Pointer if not in State.PointerCaptured?");

            _state = State.Default;
            if (OnTriggered != null)
            {
                Debug.Log("HandlerFound");
                OnTriggered.Invoke();
            }
        }


        public void PointerUpdate(LaserPointer pointer)
        {
            var hitPos = pointer.HitInfo.point;

            if (_state == State.PointerCaptured)
            {
                var distanceSinceLastHapticPush = Vector3.Distance(hitPos, _lastHapticPosition);
                if (distanceSinceLastHapticPush > MIN_HAPTIC_DISTANCE)
                {
                    pointer.Controller.TriggerHapticPulse(200);
                    _lastHapticPosition = hitPos;
                }
            }
        }

        private Vector3 _lastHapticPosition;
        private float MIN_HAPTIC_DISTANCE = 0.1f;


        public void PointerExit(LaserPointer pointer)
        {
            _renderer.material.SetColor("_EmissionColor", _originalEmissionColor);
        }

        private Color _originalEmissionColor;
        private Renderer _renderer;
        private Vector3 _cutHighlightPosition;

        private State _state;
        enum State
        {
            Default = 0,
            PointerCaptured,
        }


        public void OnPointerClick(PointerEventData eventData)
        {

        }

        public void OnSubmit(BaseEventData eventData)
        {
        }

        Button button;
    }
}
