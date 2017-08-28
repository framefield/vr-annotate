using UnityEngine;
using System.Collections.Generic;

namespace ff.vr.interaction
{
    /// <summary>
    /// This Compononent handles the interaction with Vive Controllers for teleportation, 
    /// annotations, drag-gizmos, buttons, etc.
    /// </summary>

    [RequireComponent(typeof(SteamVR_TrackedController))]
    public class InteractiveController : MonoBehaviour
    {
        public GameObject LaserPrefab;

        // This is override by derived classes
        protected virtual void Start()
        {
            Initialize();
        }

        protected void Initialize()
        {
            if (_controllerInitialized)
                return;

            _laserInstance = Instantiate(LaserPrefab);
            _laserInstance.transform.SetParent(transform, false);
            _laserPointer = _laserInstance.GetComponent<LaserPointer>();
            _laserPointer.Controller = this;

            _teleportation = GetComponent<Teleportation>();
            _audioSourceForTeleportation = GetComponent<AudioSource>();
            _controllerInitialized = true;
        }
        private bool _controllerInitialized = false;

        private AudioSource _audioSourceForTeleportation;

        protected virtual void OnEnable()
        {
            // Setup EventHandlers
            _controller = GetComponent<SteamVR_TrackedController>();
            if (_controller == null)
            {
                Debug.LogError("SteamVR_Teleporter must be on a SteamVR_TrackedController");
                return;
            }
            _controller.TriggerClicked += TriggerClickedHandler;
            _controller.TriggerUnclicked += TriggerUnclickedHandler;
            _controller.PadClicked += PadClickedHandler;
            _controller.PadUnclicked += PadUnclickedHandler;

            UpdateListofGizmoColliders();
        }

        protected virtual void OnDisable()
        {
            if (_controller == null)
                return;

            _controller.TriggerClicked -= TriggerClickedHandler;
            _controller.TriggerUnclicked -= TriggerUnclickedHandler;
            _controller.PadClicked -= PadClickedHandler;
            _controller.PadUnclicked -= PadUnclickedHandler;
        }



        public void UpdateListofGizmoColliders()
        {
            foreach (var h in FindObjectsOfType<InteractiveGizmo>())
            {
                var collider = h.GetComponent<BoxCollider>();
                if (collider != null)
                {
                    GizmoColliders.Add(collider);
                }
            }
        }

        private List<BoxCollider> GizmoColliders = new List<BoxCollider>();


        protected virtual void Update()
        {
            // Debug.Log(_state);
            // Find current hover interactive handle
            var newHoverGizmo = GetGizmoUnderController();

            if (_state == States.Default)
            {
                if (newHoverGizmo)
                {
                    newHoverGizmo.OnControllerEnter(this);
                    ActiveState = States.IsCollidingWithGizmo;
                    _currentHoverGizmo = newHoverGizmo;
                    _laserPointer.SetLaserpointerEnabled(false);
                }
            }
            else if (_state == States.IsCollidingWithGizmo)
            {
                if (newHoverGizmo == _currentHoverGizmo)
                {
                    _currentHoverGizmo.OnControllerStay(this);
                }
                else
                {
                    _currentHoverGizmo.OnControllerExit(this);
                    ActiveState = States.Default;
                    _laserPointer.SetLaserpointerEnabled(true);

                }
            }
            else if (_state == States.DraggingGizmo)
            {
                _currentHoverGizmo.OnDragUpdate(this);
            }
            else if (_state == States.PointerCapturedOnClickable || _state == States.PointerCapturedOnTeleportationTrigger)
            {
                if (newHoverGizmo != _currentHoverGizmo)
                {
                    _currentHoverGizmo.OnControllerExit(this);
                    _laserNoLongerPointsAtCapturedScreen = true;
                    if (_capturedTeleportationTrigger != null)
                        _capturedTeleportationTrigger.SetPerspectiveHighlight(false);
                }

                if (_laserNoLongerPointsAtCapturedScreen && newHoverGizmo == _currentHoverGizmo)
                {
                    _laserNoLongerPointsAtCapturedScreen = false;
                    _currentHoverGizmo.OnControllerEnter(this);
                }
            }
        }

        private bool _laserNoLongerPointsAtCapturedScreen = false;


        private void TriggerClickedHandler(object sender, ClickedEventArgs clickedEventArgs)
        {
            if (_laserPointer == null)
            {
                Debug.LogError("LaserPointer component not found for interactiveController");
                return;
            }

            if (_state == States.Default)
            {
                if (_laserPointer.PointingAt is IClickableLaserPointerTarget)
                {
                    var clickableTarget = _laserPointer.PointingAt as IClickableLaserPointerTarget;

                    clickableTarget.PointerTriggered(_laserPointer);
                    _capturedClickable = clickableTarget;
                    ActiveState = States.PointerCapturedOnClickable;
                    _laserPointer.IsLockedAtTarget = true;
                }
            }
            else if (_state == States.IsCollidingWithGizmo)
            {
                if (_currentHoverGizmo.OnDragStarted(this))
                    _state = States.DraggingGizmo;
            }
        }


        private void TriggerUnclickedHandler(object sender, ClickedEventArgs clickedEventArgs)
        {
            if (_state == States.DraggingGizmo)
            {
                _currentHoverGizmo.OnDragCompleted(this);
                ActiveState = States.Default;
            }
            else if (_state == States.PointerCapturedOnClickable)
            {
                if (_capturedClickable == null)
                {
                    Debug.LogError("Can't release pointer if not captured by Target");
                    return;
                }
                _laserNoLongerPointsAtCapturedScreen = false;
                _capturedClickable.PointerUntriggered(_laserPointer);
                _capturedClickable = null;
                ActiveState = States.Default;
                _laserPointer.IsLockedAtTarget = false;
            }

        }

        private void PadClickedHandler(object sender, ClickedEventArgs clickedEventArgs)
        {
            if (_laserPointer == null)
            {
                Debug.LogError("LaserPointer component not found for interactiveController");
                return;
            }
            if (_state == States.Default)
            {
                if (_laserPointer.PointingAt is TeleportationSpot)
                {
                    _teleportation.JumpToSpot(_laserPointer.PointingAt as TeleportationSpot);
                    if (_audioSourceForTeleportation)
                        _audioSourceForTeleportation.Play();
                }
                else if (_laserPointer.PointingAt is TeleportationZone)
                {
                    _teleportation.JumpToPosition(_laserPointer.LastHitPoint);
                    if (_audioSourceForTeleportation)
                        _audioSourceForTeleportation.Play();
                }
                else if (_laserPointer.PointingAt is AnnotationPositionTeleporter)
                {
                    _capturedTeleportationTrigger = _laserPointer.PointingAt as AnnotationPositionTeleporter;
                    _capturedTeleportationTrigger.SetPerspectiveHighlight(true);
                    _state = States.PointerCapturedOnTeleportationTrigger;
                }
            }
        }
        private void PadUnclickedHandler(object sender, ClickedEventArgs clickedEventArgs)
        {
            if (_state == States.PointerCapturedOnTeleportationTrigger && _laserPointer.PointingAt is AnnotationPositionTeleporter)
            {
                _teleportation.JumpToPosition(_capturedTeleportationTrigger.GetTeleportationTarget());
                if (_audioSourceForTeleportation)
                    _audioSourceForTeleportation.Play();
                _state = States.Default;
                _laserNoLongerPointsAtCapturedScreen = false;
                _capturedTeleportationTrigger.SetPerspectiveHighlight(false);
            }
        }

        InteractiveGizmo GetGizmoUnderController()
        {
            InteractiveGizmo newHoverGizmo = null;
            var position = transform.position;
            foreach (var collider in GizmoColliders)
            {
                if (collider.bounds.Contains(position))
                {
                    newHoverGizmo = collider.gameObject.GetComponent<InteractiveGizmo>();
                    Debug.Log(">>>> collider under controller:" + collider.gameObject, this);
                    break;
                }
            }

            return newHoverGizmo;
        }

        // ToDo: will be used later
        // void SendPositionToGizmos()
        // {
        //     var position = transform.position;
        //     foreach (var collider in GizmoColliders)
        //     {
        //         if (!collider.gameObject.activeSelf)
        //             continue;

        //         var newHoverGizmo = collider.gameObject.GetComponent<InteractiveGizmo>();
        //         newHoverGizmo.OnControllerPositionUpdate(this);
        //     }
        // }

        public void TriggerHapticPulse(ushort durationMicroSec = 500)
        {
            var controllerInput = SteamVR_Controller.Input((int)_controller.controllerIndex);
            controllerInput.TriggerHapticPulse(durationMicroSec);
        }

        private Teleportation _teleportation;
        protected LaserPointer _laserPointer;
        private GameObject _laserInstance;
        protected SteamVR_TrackedController _controller;
        private InteractiveGizmo _currentHoverGizmo;
        private IClickableLaserPointerTarget _capturedClickable;
        private AnnotationPositionTeleporter _capturedTeleportationTrigger;


        private States _state = States.Default;

        public States ActiveState
        {
            get
            {
                return _state;
            }
            private set
            {
                if (value == _state)
                {
                    Debug.LogWarning("Setting state " + value + " again?");
                    return;
                }
                _state = value;
            }
        }

        public enum States
        {
            Default = 0,
            IsCollidingWithGizmo,
            DraggingGizmo,
            PointerCapturedOnClickable,
            PointerCapturedOnTeleportationTrigger
        }
    }
}
