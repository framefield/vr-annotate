using UnityEngine;

public class ControllerTutorial : MonoBehaviour
{

    [SerializeField] Texture[] ImagesForTutorialSteps;
    [SerializeField] AnimationCurve _sizeOverStepProgress;
    [SerializeField] AnimationCurve _rotationOverStepProgress;

    [Header("---- Internal prefab references -------")]
    public MeshRenderer _tutorialObject;
    public TrackpadUI _trackpadUI;

    void Awake()
    {
        _controller = GetComponentInParent<SteamVR_TrackedController>();
    }

    void Update()
    {
        AnimateBetweenSteps();
    }


    #region event handlers
    void OnEnable()
    {
        _trackpadUI.ControllerModeChangedEvent += ControllerModeChangedHandler;
        _controller.PadClicked += MenuButtonClickedHandler;
        Show();
    }

    void OnDisable()
    {
        _trackpadUI.ControllerModeChangedEvent -= ControllerModeChangedHandler;
        _controller.PadClicked -= MenuButtonClickedHandler;
    }

    void ControllerModeChangedHandler(object s, ControllerModeChangedEventArgs e)
    {
        var tutorialIsActive = e.NewMode == TrackpadUI.ControllerEditMode.Tutorial;
        var tutorialWasActive = e.OldMode == TrackpadUI.ControllerEditMode.Tutorial;

        if (tutorialIsActive != tutorialWasActive)
        {
            if (tutorialIsActive)
            {
                Show();
            }
            else
            {
                Hide();
            }
        }
    }

    void MenuButtonClickedHandler(object sender, ClickedEventArgs e)
    {
        if (!IsHidden)
        {
            ShowNextStep();
        }
    }
    #endregion

    private bool IsHidden
    {
        get { return _shownImageIndex == -1; }
    }


    #region interal implementation
    private void AnimateBetweenSteps()
    {
        var progress = (Time.time - _stepStartTime) / STEP_TRANSITION_DURATION;
        var s = IsHidden ? 0 : _sizeOverStepProgress.Evaluate(progress) * PANEL_SIZE;
        var r = _rotationOverStepProgress.Evaluate(progress);

        _tutorialObject.transform.localScale = new Vector3(s, s, s);
        var eAngles = _tutorialObject.transform.parent.localEulerAngles;
        eAngles.z = r;
        _tutorialObject.transform.parent.localEulerAngles = eAngles;

        if (_shownImageIndex != _nextImageIndex && progress > SWTICH_TRESHOLD)
        {
            SetImageForStepIndexOrHidden(_nextImageIndex);
        }
    }

    private void Show()
    {
        _stepStartTime = Time.time;
        _nextImageIndex = 0;
    }

    private void Hide()
    {
        _nextImageIndex = -1;  // Hidden;
        _stepStartTime = Time.time;
    }

    private void ShowNextStep()
    {
        _nextImageIndex++;
        _stepStartTime = Time.time;
        if (_nextImageIndex == ImagesForTutorialSteps.Length)
        {
            Hide();
            return;
        }
    }

    private void SetImageForStepIndexOrHidden(int newImageIndex)
    {
        if (ImagesForTutorialSteps.Length == 0)
            return;

        var shouldBeHidden = newImageIndex == -1;
        if (shouldBeHidden)
        {
            _tutorialObject.gameObject.SetActive(false);
        }
        else
        {
            _tutorialObject.gameObject.SetActive(true);
            _tutorialObject.material.mainTexture = ImagesForTutorialSteps[_nextImageIndex];
            _tutorialObject.material.mainTexture.mipMapBias = -1f;
        }
        _shownImageIndex = newImageIndex;
    }
    #endregion

    private int _nextImageIndex = -1;
    private int _shownImageIndex = -1;
    private float _stepStartTime;
    private const float STEP_TRANSITION_DURATION = 0.5f;
    private const float SWTICH_TRESHOLD = 0.3f;
    private const float PANEL_SIZE = 0.25f;

    protected SteamVR_TrackedController _controller;
    private TrackpadUI.ControllerEditMode _currentEditMode;
}
