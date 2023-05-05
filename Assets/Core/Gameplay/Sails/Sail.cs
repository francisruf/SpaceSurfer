using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Sail : MonoBehaviour, IWindAgent
{
    [Header("Settings")]
    [SerializeField] private SailProperties _sailProperties;
    
    [Header("Debug")]
    [SerializeField] private GameObject _windDebugPrefab;
    
    // Components
    private Animator _animator;
    private List<LineRenderer> _windDebugs = new List<LineRenderer>();

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
        IWindAgent.newAgentSubscribeRequest?.Invoke(this);
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

    public void WindUpdate(List<WindForce> windForces)
    {
        int activeRenderers = 0;

        for (int i = 0; i < windForces.Count; i++)
        {
            //Debug.DrawLine(transform.position, transform.position + (Vector3)windForces[i].direction * windForces[i].strength * 2, Color.green, Time.deltaTime);
            
            if (i >= _windDebugs.Count)
            {
                _windDebugs.Add(Instantiate(_windDebugPrefab, transform).GetComponent<LineRenderer>());
            }

            Vector3 end = transform.position + (Vector3)windForces[i].direction * windForces[i].strength * 2;
            float dot = Vector2.Dot(transform.up, windForces[i].direction);
            Color lineColor = Color.Lerp(Color.red, Color.green, dot);

            _windDebugs[i].enabled = true;
            _windDebugs[i].SetPosition(0, transform.position);
            _windDebugs[i].SetPosition(1, end);
            _windDebugs[i].startColor = lineColor;
            _windDebugs[i].endColor = lineColor;

            activeRenderers++;
        }

        for (int i = activeRenderers; i < _windDebugs.Count; i++)
        {
            _windDebugs[i].enabled = false;
        }
    }

    public Vector3 GetWorldPosition()
    {
        return transform.position;
    }
}
