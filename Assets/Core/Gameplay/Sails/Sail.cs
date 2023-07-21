using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Sail : MonoBehaviour, IWindAgent
{
    [Header("Settings")]
    //[SerializeField] private SailProperties _sailProperties;
    [SerializeField] private float _closedSpeed;
    [SerializeField] private float _raisedSpeedMultiplier;
    [SerializeField] private float _extendedSpeedMultiplier;
    [SerializeField] private float _negativeDirectionRatio = 0.25f;

    [Header("Debug")]
    [SerializeField] private bool _debugForces;
    [SerializeField] private GameObject _windDebugPrefab;
    
    // Components
    private Animator _animator;
    private List<LineRenderer> _windDebugs = new List<LineRenderer>();

    // Internal logic
    private ESailState _sailState;
    public ESailState SailState { get { return _sailState; } }
    private float _currentWindForce;

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
        return transform.up * GetCurrentSpeed();
    }

    private float GetCurrentSpeed()
    {
        switch (_sailState)
        {
            case ESailState.Closed:
                return _closedSpeed;
            case ESailState.Raised:
                return _raisedSpeedMultiplier * _currentWindForce;
            case ESailState.Extended:
                return _extendedSpeedMultiplier * _currentWindForce;
            default:
                return 0f;
        }
    }

    public void WindUpdate(List<WindForce> windForces)
    {
        _currentWindForce = 0f;
        List<float> forces = new List<float>();

        // Calculate if sail direction matches wind direction
        for (int i = 0; i < windForces.Count; i++)
        { 
            float force;
            float dot = Vector2.Dot(transform.up, windForces[i].direction);

            if (dot < 0f)
            {
                force = dot * windForces[i].strength * _negativeDirectionRatio;
            }
            else
            {
                force = dot * windForces[i].strength;
            }
            forces.Add(force);
        }
        forces.Sort(new ReverseSort());

        float multiplier = 1f;

        for (int i = 0; i < forces.Count; i++)
        {
            forces[i] *= multiplier;
            _currentWindForce += forces[i];
            multiplier /= 2f;
        }

        if (_debugForces)
            DrawDebugWindForces(windForces);
    }


    private void DrawDebugWindForces(List<WindForce> windForces)
    {
        int activeRenderers = 0;

        for (int i = 0; i < windForces.Count; i++)
        {
            float dot = Vector2.Dot(transform.up, windForces[i].direction);

            if (i >= _windDebugs.Count)
            {
                _windDebugs.Add(Instantiate(_windDebugPrefab, transform).GetComponent<LineRenderer>());
            }
            Vector3 end = transform.position + (Vector3)windForces[i].direction * windForces[i].strength * 2;

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


    public class ReverseSort : IComparer<float>
    {
        public int Compare(float a, float b)
        {
            if (a >= b)
                return -1;
            if (Mathf.Approximately(a, b))
                return 0;

            return 1;
        }
    }
}

