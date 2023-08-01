using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public abstract class PlayerController : MonoBehaviour
{
    public Action<Character> OnPossess;
    public Action<Character> OnUnpossess;

    protected Character _possessedCharacter;
    protected EPlayerControllerState _controllerState;

    protected virtual void Awake()
    {
        
    }

    public abstract void OnMove(InputValue value);

    public void Possess(Character character)
    {
        _possessedCharacter = character;
        _possessedCharacter.OnCharacterEnabled += HandleCharacterEnabled;
        _possessedCharacter.OnCharacterDisabled += HandleCharacterDisabled;
        OnPossess?.Invoke(character);
    }

    public void Unpossess()
    {
        Character unpossessed = _possessedCharacter;
        _possessedCharacter = null;
        OnUnpossess?.Invoke(unpossessed);
        _possessedCharacter.OnCharacterDisabled -= HandleCharacterDisabled;
        _possessedCharacter.OnCharacterEnabled -= HandleCharacterEnabled;
    }

    public void HandleCharacterEnabled(Character character)
    {
        SetControllerState(EPlayerControllerState.MoveCharacter);
    }

    public void HandleCharacterDisabled(Character character)
    {
        SetControllerState(EPlayerControllerState.CharacterDisabled);
    }

    public virtual void SetControllerState(EPlayerControllerState newState)
    {
        _controllerState = newState;
    }
}

public enum EPlayerControllerState
{
    CharacterDisabled,
    MoveCharacter
}
