using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherSystem : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] protected bool _debug;

    protected bool _isEnabled;
    protected LegacyWeatherGrid _weatherGrid;
    
    public virtual void Initialize(LegacyWeatherGrid grid)
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
}
