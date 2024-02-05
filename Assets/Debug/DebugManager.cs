using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DebugManager : MonoBehaviour
{
    public static Action<DebugManager> onDebugEnabled;
    public static Action<DebugManager> onDebugDisabled;

    public static DebugManager instance;

    // Properties
    [SerializeField] private bool _enableAtStart;

    // Components
    [SerializeField] private Canvas _debugCanvas;
    [SerializeField] private Canvas _notificationCanvas;

    [SerializeField] private TextMeshProUGUI _characterDataText;
    [SerializeField] private TextMeshProUGUI _notificationText;

    [SerializeField] private List<DebugText> _debugTexts;

    // Internal logic
    // TODO : Move this to the controller
    private bool _debugEnabled;

    // Game references
    private Character _playerCharacter;

    // Notification
    private IEnumerator currentNotification;


    private void OnEnable()
    {
        Character.OnPlayerCharacterEnabled += HandleNewPlayerCharacter;
    }

    private void OnDisable()
    {
        Character.OnPlayerCharacterEnabled -= HandleNewPlayerCharacter;
    }

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this.gameObject);
        else
            instance = this;

        ToggleDebug(_enableAtStart);
    }

    private void ToggleDebug(bool isEnabled)
    {
        _debugCanvas.enabled = isEnabled;
        _debugEnabled = isEnabled;

        if (_debugEnabled)
            onDebugEnabled?.Invoke(this);
        else
            onDebugDisabled?.Invoke(this);
    }

    private void HandleNewPlayerCharacter(Character character)
    {
        _playerCharacter = character;
    }

    // TODO : Move this to the player controller
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
            _playerCharacter?.ToggleDebug();

        if (Input.GetKeyDown(KeyCode.F12))
            ToggleDebug(!_debugEnabled);

        if (!_debugEnabled)
            return;

        if (_playerCharacter != null)
            _characterDataText.text = _playerCharacter.GetCharacterDebugData();
    }

    public void RequestNotification(string notification, float duration = 3f)
    {
        if (currentNotification != null)
            StopCoroutine(currentNotification);

        currentNotification = ShowNotification(notification, duration);
        StartCoroutine(currentNotification);
    }

    private IEnumerator ShowNotification(string notification, float duration)
    {
        float timer = 0f;

        _notificationCanvas.enabled = true;
        _notificationText.text = notification;

        while (timer < duration)
        {
            yield return null;
            timer += Time.deltaTime;
        }

        _notificationCanvas.enabled = false;
        currentNotification = null;
    }

    public void EnableDebugText(int index, string debugText)
    {
        if (index >= _debugTexts.Count)
            return;

        _debugTexts[index].DisplayText(debugText);
    }

    public void DisableDebugText(int index)
    {
        if (index >= _debugTexts.Count)
            return;

        _debugTexts[index].HideText();
    }
}
