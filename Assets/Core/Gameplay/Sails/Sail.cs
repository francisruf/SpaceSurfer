using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sail : MonoBehaviour, IGridActor<WeatherTileData>
{
    public static Action<IGridActor<WeatherTileData>> onNewSail;

    private WeatherTileData _currentWeatherTile;
    private List<WeatherSystem> _weatherSystems = new List<WeatherSystem>();
    private List<WeatherModifier> _weatherModifiers = new List<WeatherModifier>();

    public void AssignCurrentTileData(WeatherTileData tileData)
    {
        _currentWeatherTile = tileData;
        _weatherSystems = tileData.GetWeatherSystems();

        for (int i = 0; i < _weatherSystems.Count; i++)
        {
            _weatherModifiers.Add(_weatherSystems[i].GetWeatherModifier(this.transform.position));
        }

        for (int i = 0; i < _weatherModifiers.Count; i++)
        {
            Vector2 start = transform.position;
            Vector2 end = transform.position + (Vector3)_weatherModifiers[i].direction * _weatherModifiers[i].strength;

            Debug.DrawLine(start, end, Color.green, Time.deltaTime);
        }
    }

    public Vector3 GetWorldPosition()
    {
        return this.transform.position;
    }

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
        onNewSail(this);
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
