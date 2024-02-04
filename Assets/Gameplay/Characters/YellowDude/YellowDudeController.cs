using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(DebugCharacter))]
public class YellowDudeController : PlayerController
{
    public static Action<PlayerController> requestOpenDevice;

    protected DebugCharacter _character;
    private FieldOfView _fieldOfView;

    protected override void Awake()
    {
        base.Awake();
        _fieldOfView = GetComponentInChildren<FieldOfView>();
    }

    protected virtual void Update()
    {
        _character = GetComponent<DebugCharacter>();
    }

    public override void OnMove(InputValue value)
    {
        base.OnMove(value);
        Vector2 input = value.Get<Vector2>();
        _character?.RequestMove(input);
        _fieldOfView?.SetOrigin(transform.position);
    }

    public override void OnAim(Vector2 aim)
    {
        base.OnAim(aim);
        _fieldOfView?.SetAimDirection(aim);
        _fieldOfView?.UpdateFieldOfView();
    }

    public void OnOpenDevice(InputValue value)
    {
        requestOpenDevice?.Invoke(this);
    }
}
