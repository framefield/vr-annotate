using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlUiFeatures : MonoBehaviour
{
    [SerializeField] string[] features = new string[] { "Feature1", "Feature2" };
    [SerializeField] TMPro.TextMeshProUGUI featureText;
    [SerializeField] AnimationCurve PositionFromProgress;

    void Start()
    {

    }

    void Update()
    {
        var progress = (Time.time - _featureStartTime) / FEATURE_DURATION;
        var x = PositionFromProgress.Evaluate(progress);
        var p = featureText.transform.localPosition;
        p.x = x;
        featureText.transform.localPosition = p;

        if (progress > 1)
        {
            ShowNextFeature();
        }
    }


    private void ShowNextFeature()
    {
        if (features.Length == 0)
            return;

        _featureIndex++;
        featureText.text = features[_featureIndex % features.Length];
        _featureStartTime = Time.time;
    }

    private const float FEATURE_DURATION = 5;
    private int _featureIndex = 0;
    private float _featureStartTime;

}
