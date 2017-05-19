using UnityEngine;
using System.Collections;
using System;

public class TrackpadButtonUI : MonoBehaviour
{
    [System.Serializable]
    public class EditModeDefinition
    {
        public ControllerButtons Mode;
        public GameObject Icon;
    }

    public enum ControllerButtons
    {
        Up,
        Right,
        Down,
        Left,
        Center,
        None,
    }

    public delegate void TrackpadUIButtonClicked(object s, ControllerButtons buttonPressed);
    public event TrackpadUIButtonClicked TrackpadUIButtonClickedEvent;

    public SteamVR_TrackedController Controller;

    public EditModeDefinition[] Modes;

    [Header("--- Internal Prefab Referecens----")]
    public GameObject ActiveModePlane;
    public GameObject ActiveTutorialQuad;

    [HideInInspector]
    public ControllerButtons CurrentlyPressedButton;


    void Start()
    {
        if (Controller == null)
        {
            Controller = GetComponentInParent<SteamVR_TrackedController>();
            if (Controller == null)
            {
                Debug.LogWarning("Couldn't find controller in parent.", this);
            }
        }

        foreach (MeshRenderer r in GetComponentsInChildren<MeshRenderer>())
        {
            r.material.mainTexture.mipMapBias = -1;
        }
    }

    void Update()
    {
        TryAttachingController();
        if (!_isAttachedToController)
            return;

        UpdateModeIndicator();
    }


    private void UpdateModeIndicator()
    {
        var padX = Controller.controllerState.rAxis0.x;
        var padY = Controller.controllerState.rAxis0.y;

        var shownMode = _padTouched ? GetModeForPadPosition(padX, padY) : CurrentlyPressedButton;
        if (shownMode != _lastEditMode)
        {
            TriggerHapticPuse();
            _lastEditMode = shownMode;
        }

        //Debug.Log("ShownMode:" + showMode + " / " + CurrentEditMode);

        if (shownMode == ControllerButtons.Center)
        {
            if (ActiveTutorialQuad)
            {
                ActiveTutorialQuad.SetActive(true);
            }
            ActiveModePlane.SetActive(false);
        }
        else
        {
            if (ActiveTutorialQuad)
            {
                ActiveTutorialQuad.SetActive(false);
            }

            ActiveModePlane.SetActive(true);
            var newRotation = ((int)shownMode) * 90;
            activeModeRotationDamped = Mathf.LerpAngle(newRotation, activeModeRotationDamped, FRICTION);
            ActiveModePlane.transform.localEulerAngles = new Vector3(0, 0, -activeModeRotationDamped + 180);
        }
    }

    private void TriggerHapticPuse()
    {
        var controllerInput = SteamVR_Controller.Input((int)Controller.controllerIndex);
        controllerInput.TriggerHapticPulse(400);
    }

    private void TryAttachingController()
    {
        if (_isAttachedToController)
            return;

        if (Controller == null)
        {
            Debug.LogWarning("controller not connected");
            return;
        }

        Controller.PadTouched += OnPadTouched;
        Controller.PadUntouched += OnPadUntouched;
        Controller.PadClicked += OnPadClicked;
        Controller.PadUnclicked += OnPadUnclicked;
        _isAttachedToController = true;
    }

    #region event-handlers
    private void OnPadClicked(object sender, ClickedEventArgs clickedEventArgs)
    {
        var pressedButton = GetModeForPadPosition(clickedEventArgs.padX, clickedEventArgs.padY);

        if (pressedButton == CurrentlyPressedButton)
            return;

        if (TrackpadUIButtonClickedEvent != null)
        {
            TrackpadUIButtonClickedEvent(this, pressedButton);
        }
        CurrentlyPressedButton = pressedButton;
    }

    private void OnPadUnclicked(object sender, ClickedEventArgs clickedEventArgs)
    {
        CurrentlyPressedButton = ControllerButtons.None;
    }

    private void OnPadTouched(object sender, ClickedEventArgs clickedEventArgs)
    {
        _padTouched = true;
    }

    private void OnPadUntouched(object sender, ClickedEventArgs clickedEventArgs)
    {
        CurrentlyPressedButton = ControllerButtons.None;
        _padTouched = false;
    }
    #endregion



    #region implementation

    private ControllerButtons GetModeForPadPosition(float padX, float padY)
    {
        var isInCenter = new Vector3(padX, padY).magnitude < CENTER_RADIUS;
        if (isInCenter)
        {
            return ControllerButtons.Center;
        }

        var deg = Mathf.Atan2(padX, padY);
        var r = (deg + Mathf.PI) / Mathf.PI / 2.0f + 0.5f;

        var rRounded = Mathf.Floor(r * Modes.Length + 0.5f);
        rRounded = rRounded % Modes.Length;

        return Modes[(int)rRounded].Mode;
    }
    #endregion

    private ControllerButtons _lastEditMode;
    private float activeModeRotationDamped = 0;
    private const float FRICTION = 0.8f;
    private const float CENTER_RADIUS = 0.35f;
    private ControllerButtons _hoverMode;
    private bool _padTouched = false;

    bool _isAttachedToController = false;
}

// public class ControllerModeChangedEventArgs : EventArgs
// {
//     public TrackpadButtonUI.ControllerButtons NewMode;
//     public TrackpadButtonUI.ControllerButtons OldMode;

//     public ControllerModeChangedEventArgs(TrackpadButtonUI.ControllerButtons newMode, TrackpadButtonUI.ControllerButtons oldMode)
//     {
//         NewMode = newMode;
//         OldMode = oldMode;
//     }
// }
