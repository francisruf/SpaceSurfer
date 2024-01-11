using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherManager : MonoBehaviour
{
    public WeatherSystem[] debugStartingWeather;
    //private WeatherSystem[] _allWeatherSystems;

    // Components
    private LegacyWeatherGrid _weatherGrid;

    private Character _player;

    private void OnEnable()
    {
        Character.OnPlayerCharacterEnabled += HandleNewPlayerCharacter;
    }

    private void OnDisable()
    {
        Character.OnPlayerCharacterEnabled -= HandleNewPlayerCharacter;
    }

    private void Awake()
    {
        _weatherGrid = FindObjectOfType<LegacyWeatherGrid>();

        if (_weatherGrid == null)
            Debug.LogError("Weather manager could not find weather grid and will not function properly.");
    }

    private void Start()
    {
        InitializeWeatherSystems();
    }

    private void HandleNewPlayerCharacter(Character character)
    {
        _player = character;
    }

    private void InitializeWeatherSystems()
    {
        for (int i = 0; i < debugStartingWeather.Length; i++)
        {
            //debugStartingWeather[i].Initialize(_weatherGrid);
        }
    }
}