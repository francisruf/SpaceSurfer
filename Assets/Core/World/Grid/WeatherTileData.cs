using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherTileData
{
    private List<WeatherSystem> _weatherSystems = new List<WeatherSystem>();
    private List<WeatherModifier> _weatherModifiers = new List<WeatherModifier>();



    // TODO : This doesnt work since it doesnt clear the previous weather modifiers in the frame.

    public List<WeatherModifier> GetWeatherModifiers()
    {
        return _weatherModifiers;
    }

    public void AddWeatherModifier(WeatherModifier modifier)
    {
        _weatherModifiers.Add(modifier);
    }

    public void RemoveWeatherModifier(WeatherModifier modifier)
    {
        _weatherModifiers.Remove(modifier);
    }

    public List<WeatherSystem> GetWeatherSystems()
    { 
       return _weatherSystems;
    }

    public void AddWeatherSystem(WeatherSystem weather)
    {
        _weatherSystems.Add(weather);
    }

    public void RemoveWeatherSystem(WeatherSystem weather)
    {
        _weatherSystems.Remove(weather);
    }

}
