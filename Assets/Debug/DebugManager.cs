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
    public TextMeshProUGUI _characterDataText;

    // Internal logic
    // TODO : Move this to the controller
    private bool _debugEnabled;
    private int _debugWindDirection;

    // Game references
    private Character _playerCharacter;

    private void OnEnable()
    {
        Character.OnPlayerCharacterInstantiate += HandleNewPlayerCharacter;
    }

    private void OnDisable()
    {
        Character.OnPlayerCharacterInstantiate -= HandleNewPlayerCharacter;
    }

    private void Awake()
    {
        _debugCanvas = GetComponentInChildren<Canvas>();
        _debugCanvas.enabled = _enableAtStart;
    }

    private void HandleNewPlayerCharacter(Character character)
    {
        _playerCharacter = character;
    }

    private void ToggleDebugCanvas(bool enableDebug)
    {
        _debugEnabled = enableDebug;
        _debugCanvas.enabled = enableDebug;
    }


    // TODO : Move this to the player controller
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
            _playerCharacter?.ToggleDebug();

        if (Input.GetKeyDown(KeyCode.F12))
            ToggleDebugCanvas(!_debugEnabled);

        if (!_debugEnabled)
            return;

        if (_playerCharacter != null)
            _characterDataText.text = _playerCharacter.GetCharacterDebugData();
    }
}
