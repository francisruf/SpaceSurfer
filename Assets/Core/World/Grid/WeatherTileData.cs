using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherTileData
{
    private List<WeatherSystem> _weatherSystems = new List<WeatherSystem>();

    public void AddWeatherSystem(WeatherSystem weather)
    {
        _weatherSystems.Add(weather);
    }
}
