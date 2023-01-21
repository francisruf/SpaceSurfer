using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(DebugCharacter))]
public class DebugCharacterController : PlayerController
{
    private DebugCharacter _character;

    private void Awake()
    {
        _character = GetComponent<DebugCharacter>();
    }

    public override void OnMove(InputValue value)
    {
        _character?.RequestMove(value.Get<Vector2>());
    }
}
