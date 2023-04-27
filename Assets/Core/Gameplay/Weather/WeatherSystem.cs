using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeatherSystem : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] protected bool _debug;

    protected bool _isEnabled;
    protected WeatherGrid _weatherGrid;
    
    public virtual void Initialize(WeatherGrid grid)
    {
        _weatherGrid = grid;
    }

    protected virtual void EnableSystem()
    {
        _isEnabled = true;
    }

    protected virtual void DisableSystem()
    {
        _isEnabled = false;
    }

    public abstract WeatherModifier GetWeatherModifier(Vector3 worldPos);
}

public struct WeatherModifier
{
    public float strength;
    public Vector2 direction;

    public WeatherModifier(float strength, Vector2 direction)
    {
        this.strength = strength;
        this.direction = direction;
    }
}
