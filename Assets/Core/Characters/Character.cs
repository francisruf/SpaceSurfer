using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
    public static Action<Character> OnPlayerCharacterInstantiate;
    public static Action<Character> OnPlayerWorldPositionUpdate;

    [Header("Game Settings")]
    [SerializeField] protected bool _playerCharacter = true;

    [Header("Debug Settings")]
    [SerializeField] protected bool _startWithDebugEnabled = false;
    protected bool _debugEnabled = false;

    public abstract void RequestMove(Vector2 direction);
    public virtual void ToggleDebug()
    {
        _debugEnabled = !_debugEnabled;
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
            InitializeCharacter(_playerCharacter);
    }

    public void InitializeCharacter(bool isPlayerCharacter)
    {
        _playerCharacter = isPlayerCharacter;

        if (_playerCharacter)
        {
            if (OnPlayerCharacterInstantiate != null)
                OnPlayerCharacterInstantiate(this);

            _debugEnabled = _startWithDebugEnabled;
            StartCoroutine(UpdateWorldPosition());
        }
    }

    // Send player world position update 10 times per sec
    protected IEnumerator UpdateWorldPosition()
    {
        while (true)
        {
            if (OnPlayerWorldPositionUpdate != null)
                OnPlayerWorldPositionUpdate(this);

            yield return new WaitForSeconds(0.1f);
        }
    }
}
