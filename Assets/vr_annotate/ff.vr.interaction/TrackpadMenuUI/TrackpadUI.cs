using UnityEngine;
using System.Collections;
using System;

public class TrackpadUI : MonoBehaviour
{
    [System.Serializable]
    public class EditModeDefinition
    {
        public ControllerEditMode Mode;
        public GameObject Icon;
    }

    public enum ControllerEditMode
    {
        MoveAndRotate,
        Duplicate,
        Delete,
        Switch,
        Tutorial,
    }

    public delegate void ControllerModeChanged(object s, ControllerModeChangedEventArgs e);
    public event ControllerModeChanged ControllerModeChangedEvent;

    public SteamVR_TrackedController Controller;

    public EditModeDefinition[] Modes;

    [Header("--- Internal Prefab Referecens----")]
    public GameObject ActiveModePlane;
    public GameObject ActiveTutorialQuad;

    [HideInInspector]
    public ControllerEditMode CurrentEditMode;


    void Awake()
    {
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

        var shownMode = _padTouched ? GetModeForPadPosition(padX, padY) : CurrentEditMode;
        if (shownMode != _lastEditMode)
        {
            TriggerHapticPuse();
            _lastEditMode = shownMode;
        }

        //Debug.Log("ShownMode:" + showMode + " / " + CurrentEditMode);

        if (shownMode == ControllerEditMode.Tutorial)
        {
            ActiveTutorialQuad.SetActive(true);
            ActiveModePlane.SetActive(false);
        }
        else
        {
            ActiveTutorialQuad.SetActive(false);
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


    ControllerEditMode _lastEditMode;


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
        _isAttachedToController = true;
    }

    #region event-handlers
    private void OnPadClicked(object sender, ClickedEventArgs clickedEventArgs)
    {
        var newEditMode = GetModeForPadPosition(clickedEventArgs.padX, clickedEventArgs.padY);

        if (newEditMode == CurrentEditMode)
            return;

        if (ControllerModeChangedEvent != null)
        {
            ControllerModeChangedEvent(this, new ControllerModeChangedEventArgs(newEditMode, CurrentEditMode));
        }
        CurrentEditMode = newEditMode;
    }


    private void OnPadTouched(object sender, ClickedEventArgs clickedEventArgs)
    {
        _padTouched = true;
    }

    private void OnPadUntouched(object sender, ClickedEventArgs clickedEventArgs)
    {
        _padTouched = false;
    }
    #endregion



    #region implementation

    private ControllerEditMode GetModeForPadPosition(float padX, float padY)
    {
        var isInCenter = new Vector3(padX, padY).magnitude < CENTER_RADIUS;
        if (isInCenter)
        {
            return ControllerEditMode.Tutorial;
        }

        var deg = Mathf.Atan2(padX, padY);
        var r = (deg + Mathf.PI) / Mathf.PI / 2.0f + 0.5f;

        var rRounded = Mathf.Floor(r * Modes.Length + 0.5f);
        rRounded = rRounded % Modes.Length;

        return Modes[(int)rRounded].Mode;
    }
    #endregion

    private float activeModeRotationDamped = 0;
    private const float FRICTION = 0.8f;
    private const float CENTER_RADIUS = 0.35f;
    private ControllerEditMode _hoverMode;
    private bool _padTouched = false;

    bool _isAttachedToController = false;

}

public class ControllerModeChangedEventArgs : EventArgs
{
    public TrackpadUI.ControllerEditMode NewMode;
    public TrackpadUI.ControllerEditMode OldMode;

    public ControllerModeChangedEventArgs(TrackpadUI.ControllerEditMode newMode, TrackpadUI.ControllerEditMode oldMode)
    {
        NewMode = newMode;
        OldMode = oldMode;
    }
}
