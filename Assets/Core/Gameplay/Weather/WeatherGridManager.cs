using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherGridManager : MonoBehaviour
{
    private WeatherSystem[] _allWeatherSystems;
    private WeatherSystem[,] _weatherGrid;

    private void Start()
    {
        InitializeWeatherSystems();
    }

    private void InitializeWeatherSystems()
    {
        _allWeatherSystems = FindObjectsOfType<WeatherSystem>();
        foreach (var weather in _allWeatherSystems)
        {
            weather.Initialize(this);
        }
    }

    public void AssignWeatherData(Vector2Int[] coords, WeatherSystem weather)
    {
        int length = coords.Length;

        for (int i = 0; i < length; i++)
        {
            AssignWeatherData(coords[i].x, coords[i].y, weather);
        }
    }

    public void AssignWeatherData(int coordX, int coordY, WeatherSystem weather)
    {
        // TODO : Send updates, don't update if the same, etc.
        _weatherGrid[coordX, coordY] = weather;
    }
}
