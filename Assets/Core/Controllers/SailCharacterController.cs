using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(SailCharacter))]
public class SailCharacterController : PlayerController
{
    // Components
    private SailCharacter _sailCharacter;

    private void Awake()
    {
        _sailCharacter = GetComponent<SailCharacter>();
    }

    public override void OnMove(InputValue value)
    {
        _sailCharacter.RequestMove(value.Get<Vector2>());
    }

    public void OnSailRotation(InputValue value)
    {
        _sailCharacter.RequestSailRotation(value.Get<Vector2>());
    }

    public void OnSailRotation1D(InputValue value)
    {
        _sailCharacter.RequestSailRotation1D(value.Get<float>());
    }

    public void OnSailOpen(InputValue value)
    {
        _sailCharacter.RequestSailOpen();
    }

    public void OnSailClose(InputValue value)
    {
        _sailCharacter.RequestSailClose();
    }
}
