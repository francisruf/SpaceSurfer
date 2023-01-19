using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DebugManager : MonoBehaviour
{
    public static Action<int> OnWindDirectionRequest;

    // Properties
    [SerializeField] private bool _enableAtStart;

    // Components
    private Canvas _debugCanvas;
    public TextMeshProUGUI _windDirectionText;

    // Internal logic
    // TODO : Move this to the controller
    private bool _debugEnabled;
    private int _debugWindDirection;

    private void Awake()
    {
        _debugCanvas = GetComponentInChildren<Canvas>();
        _debugCanvas.enabled = _enableAtStart;
    }

    private void ToggleDebugCanvas(bool enableDebug)
    {
        _debugEnabled = enableDebug;
        _debugCanvas.enabled = enableDebug;
    }


    // TODO : Move this to the player controller
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F12))
            ToggleDebugCanvas(!_debugEnabled);

        if (!_debugEnabled)
            return;
    }

    // TODO : This should read the value of WindController instead of assigning it.
    public void HandleWindSliderUpdate(System.Single sliderValue)
    {
        _debugWindDirection = (int)sliderValue % 360;
        _windDirectionText.text = "Wind : " + _debugWindDirection.ToString();

        if (OnWindDirectionRequest != null)
            OnWindDirectionRequest(_debugWindDirection);
    }
}
