using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class WeatherTile
{
    private List<WeatherSystem> _weatherSystems = new List<WeatherSystem>();
    public bool HasWeatherSystem { get { return _hasWeatherSystems; } }
    private bool _hasWeatherSystems;

    public void AddWeatherSystems(WeatherSystem weather)
    {
        _weatherSystems.Add(weather);
        _hasWeatherSystems = true;
    }

    public void AddWeatherSystems(List<WeatherSystem> weatherSystems)
    {
        foreach (var weather in weatherSystems)
        {
            _weatherSystems.Add(weather);
        }
        _hasWeatherSystems = true;
    }



}
