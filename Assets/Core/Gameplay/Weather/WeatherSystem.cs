using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherSystem : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] protected bool _enableOnInitialize;
    
    protected WeatherGridManager _weatherGridManager;
    protected IEnumerator _weatherUpdate;
    protected bool _isEnabled;
    

    public virtual void Initialize(WeatherGridManager manager)
    {
        _weatherGridManager = manager;

        if (_enableOnInitialize)
        {
            _weatherUpdate = UpdateWeather();
            StartCoroutine(_weatherUpdate);
        }    
    }

    protected virtual void EnableSystem()
    {
        if (_isEnabled)
            return;

        if (_weatherUpdate != null)
            return;

        _weatherUpdate = UpdateWeather();
        StartCoroutine(_weatherUpdate);
    }

    protected virtual void DisableSystem()
    {
        if (!_isEnabled)
            return;

        if (_weatherUpdate == null)
            StopCoroutine(_weatherUpdate);

        _weatherUpdate = null;
    }

    protected virtual IEnumerator UpdateWeather()
    {
        yield return null;
    }

}
