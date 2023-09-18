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
    protected InteractionController _interactionController;

    protected bool _canMove;
    public bool CanMove { get { return _canMove; } }
    protected bool _canInteract;
    public bool CanInteract { get { return _canInteract; } }

    protected virtual void Awake()
    {
        _interactionController = GetComponent<InteractionController>();
        if (_interactionController == null)
            _interactionController = GetComponentInChildren<InteractionController>();
    }

    public virtual void OnMove(InputValue value)
    {
        if (!_canMove)
            return;
    }

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
        SetControllerState(EPlayerControllerState.MoveCharacter, true, true);
    }

    public void HandleCharacterDisabled(Character character)
    {
        SetControllerState(EPlayerControllerState.CharacterDisabled, false, false);
    }

    public virtual void SetControllerState(EPlayerControllerState newState, bool canMove, bool canInteract)
    {
        _canMove = canMove;
        _canInteract = canInteract;
        _controllerState = newState;

        if (_interactionController != null)
            _interactionController.SetCanInteract(_canInteract);
    }

    public virtual void SetCanInteract(bool canInteract)
    {
        _canInteract = canInteract;

        if (_interactionController != null)
            _interactionController.SetCanInteract(_canInteract);
    }
}

public enum EPlayerControllerState
{
    CharacterDisabled,
    MoveCharacter
}
