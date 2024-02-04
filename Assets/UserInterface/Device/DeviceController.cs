using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class DeviceController : MonoBehaviour
{
    private PlayerInput _playerInput;
    private PlayerController _previousController;
    private Animator _animatorController;
    private EventSystem _eventSystem;

    private ResonatorButton[] _resonatorButtons;
    private ResonatorButton _currentResonatorButton;

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _animatorController = GetComponent<Animator>();
        _resonatorButtons = GetComponentsInChildren<ResonatorButton>();
        _eventSystem = EventSystem.current; // get the current event system
    }

    private void OnEnable()
    {
        YellowDudeController.requestOpenDevice += OpenDevice;

        foreach (var btn in _resonatorButtons)
            btn.onButtonSelected += OnResonatorButtonSelected;
    }

    private void OnDisable()
    {
        YellowDudeController.requestOpenDevice -= OpenDevice;

        foreach (var btn in _resonatorButtons)
            btn.onButtonSelected -= OnResonatorButtonSelected;
    }

    private void OnResonatorButtonSelected(ResonatorButton resonatorButton)
    {
        _currentResonatorButton = resonatorButton;
    }

    private void OpenDevice(PlayerController previousController)
    {
        _previousController = previousController;
        _previousController.DisableInput();

        if (_currentResonatorButton == null)
            _currentResonatorButton = _resonatorButtons[0];

        _eventSystem.SetSelectedGameObject(_currentResonatorButton.gameObject);
        _playerInput.enabled = true;
        _animatorController.SetBool("IsOpen", true);

        Debug.Log("Device Controller Input Enabled!");
    }

    private void CloseDevice()
    {
        _animatorController.SetBool("IsOpen", false);
        _playerInput.enabled = false;
        _previousController.EnableInput();
        _previousController = null;

        Debug.Log("Device Controller Input Disabled!");
    }

    public void OnCloseDevice(InputValue inputValue)
    {
        CloseDevice();
    }

    public void OnTest()
    {
        Debug.Log("Device says : TEST!");
    }

    public void OnSelectGlyph(InputValue inputValue)
    {
        if (inputValue.Get<Vector2>().x > 0.1f)
            _currentResonatorButton?.SelectRightGlyph();
        else
            _currentResonatorButton?.SelectLeftGlyph();
    }
}
