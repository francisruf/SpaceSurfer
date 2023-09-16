using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(DebugCharacter))]
public class DebugCharacterController : PlayerController
{
    private DebugCharacter _character;



    protected override void Awake()
    {
        base.Awake();

    }

    protected virtual void Update()
    {
        _character = GetComponent<DebugCharacter>();
    }

    public override void OnMove(InputValue value)
    {
        Vector2 input = value.Get<Vector2>();
        _character?.RequestMove(input);
    }
}
