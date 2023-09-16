using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
    public static Action<Character> OnPlayerCharacterEnabled;
    public static Action<Character> OnPlayerCharacterDisabled;

    public Action<Character> OnCharacterEnabled;
    public Action<Character> OnCharacterDisabled;

    public static Action<Character> OnPlayerWorldPositionUpdate;

    public Action<Vector2> OnDirectionFlip;

    [Header("Game Settings")]
    [SerializeField] protected bool _playerCharacter = true;

    [Header("Debug Settings")]
    [SerializeField] protected bool _startWithDebugEnabled = false;
    protected bool _debugEnabled = false;

    public abstract void RequestMove(Vector2 direction);

    private IEnumerator _worldPositionUpdate;

    public virtual void ToggleDebug()
    {
        SetDebugEnabled(!_debugEnabled);
    }

    public virtual void SetDebugEnabled(bool enableDebug)
    {
        _debugEnabled = enableDebug;
        Debug.Log("Character Debug : " + _debugEnabled);
    }

    public virtual string GetCharacterDebugData()
    {
        return "<size=150%>No Character Data</size>";
    }

    protected virtual void Start()
    {
        // Initialize Character in scenes without GameManager
        if (GameManager.instance == null)
            EnableCharacter(_playerCharacter);
    }

    public virtual void EnableCharacter(bool isPlayerCharacter)
    {
        _playerCharacter = isPlayerCharacter;

        if (_playerCharacter)
        {
            _worldPositionUpdate = UpdateWorldPosition();
            StartCoroutine(_worldPositionUpdate);

            if (_startWithDebugEnabled)
                SetDebugEnabled(true);

            OnPlayerCharacterEnabled?.Invoke(this);
        }

        OnCharacterEnabled?.Invoke(this);

        if (_debugEnabled)
            Debug.Log("Character " + this.gameObject.name + " ENABLED.");
    }

    public virtual void DisableCharacter()
    {
        if (_worldPositionUpdate != null)
        {
            StopCoroutine(_worldPositionUpdate);
            _worldPositionUpdate = null;
        }

        OnCharacterDisabled?.Invoke(this);

        if (_playerCharacter)
            OnPlayerCharacterDisabled?.Invoke(this);

        if (_debugEnabled)
            Debug.Log("Character " + this.gameObject.name + " DISABLED.");
    }

    // Send player world position update 10 times per sec
    protected IEnumerator UpdateWorldPosition()
    {
        while (true)
        {
            OnPlayerWorldPositionUpdate?.Invoke(this);

            yield return new WaitForSeconds(0.1f);
        }
    }

    public void Teleport(Vector3 position, Quaternion rotation)
    {
        transform.position = position;
        transform.rotation = rotation;
    }

}
