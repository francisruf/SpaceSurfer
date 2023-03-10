using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sail : MonoBehaviour
{
    // Settings
    [SerializeField] private SailProperties _sailProperties;

    // Components
    private Animator _animator;

    // Internal logic
    private ESailState _sailState;
    public ESailState SailState { get { return _sailState; } }

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        ChangeSailState(ESailState.Closed);
    }

    public void RequestSailOpen()
    {
        int newState = Mathf.Clamp(((int)_sailState) + 1, 0, 2);
        ChangeSailState((ESailState)newState);
    }

    public void RequestSailClose()
    {
        int newState = Mathf.Clamp(((int)_sailState) - 1, 0, 2);
        ChangeSailState((ESailState)newState);
    }
    public void ChangeSailState(ESailState newState)
    {
        if (newState == _sailState)
            return;

        _sailState = newState;
        _animator.SetInteger("SailState", (int)_sailState);
    }

    public Vector2 GetMoveVector()
    {
        return transform.up* GetCurrentSpeed() * Time.fixedDeltaTime;
    }

    private float GetCurrentSpeed()
    {
        switch (_sailState)
        {
            case ESailState.Closed:
                return _sailProperties.closedSpeed;
            case ESailState.Raised:
                return _sailProperties.raisedSpeed;
            case ESailState.Extended:
                return _sailProperties.extendedSpeed;
            default:
                return 0f;
        }
    }
}
