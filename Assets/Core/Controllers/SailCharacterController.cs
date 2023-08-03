using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(SailCharacter))]
public class SailCharacterController : PlayerController
{
    // Components
    private SailCharacter _sailCharacter;

    protected override void Awake()
    {
        base.Awake();
        _sailCharacter = GetComponent<SailCharacter>();
    }

    public override void OnMove(InputValue value)
    {
        if (_possessedCharacter == null)
            return;

        switch (_controllerState)
        {
            case EPlayerControllerState.CharacterDisabled:
                break;
            case EPlayerControllerState.MoveCharacter:
                _sailCharacter.RequestMove(value.Get<Vector2>());
                break;
            default:
                break;
        }
    }

    public void OnSailRotation(InputValue value)
    {
        if (_possessedCharacter == null)
            return;

        switch (_controllerState)
        {
            case EPlayerControllerState.CharacterDisabled:
                break;
            case EPlayerControllerState.MoveCharacter:
                _sailCharacter.RequestSailRotation(value.Get<Vector2>());
                break;
            default:
                break;
        }
    }

    public void OnSailRotation1D(InputValue value)
    {
        if (_possessedCharacter == null)
            return;

        switch (_controllerState)
        {
            case EPlayerControllerState.CharacterDisabled:
                break;
            case EPlayerControllerState.MoveCharacter:
                _sailCharacter.RequestSailRotation1D(value.Get<float>());
                break;
            default:
                break;
        }
    }

    public void OnSailOpen(InputValue value)
    {
        if (_possessedCharacter == null)
            return;

        switch (_controllerState)
        {
            case EPlayerControllerState.CharacterDisabled:
                break;
            case EPlayerControllerState.MoveCharacter:
                _sailCharacter.RequestSailOpen();
                break;
            default:
                break;
        }
    }

    public void OnSailClose(InputValue value)
    {
        if (_possessedCharacter == null)
            return;

        switch (_controllerState)
        {
            case EPlayerControllerState.CharacterDisabled:
                break;
            case EPlayerControllerState.MoveCharacter:
                _sailCharacter.RequestSailClose();
                break;
            default:
                break;
        }
    }

    public void OnSailPressure(InputValue value)
    {
        if (_possessedCharacter == null)
            return;

        switch (_controllerState)
        {
            case EPlayerControllerState.CharacterDisabled:
                break;
            case EPlayerControllerState.MoveCharacter:
                _sailCharacter.RequestSailPressure(value.Get<float>());
                break;
            default:
                break;
        }
    }

    public void OnBrake(InputValue value)
    {
        if (_possessedCharacter == null)
            return;

        switch (_controllerState)
        {
            case EPlayerControllerState.CharacterDisabled:
                break;
            case EPlayerControllerState.MoveCharacter:
                _sailCharacter.RequestBraking(value.Get<float>());
                break;
            default:
                break;
        }
    }
}
