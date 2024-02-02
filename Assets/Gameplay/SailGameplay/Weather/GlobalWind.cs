using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalWind : WeatherSystem
{
    // Components
    private NoiseRenderer _windRenderer;

    [Header("Settings")]
    [SerializeField] private NoiseSettings _noiseSettings;
    [SerializeField] private Vector2 _direction;
    [SerializeField] private float _windSpeed = 1f;

    // Private variables
    private float[,] _noiseMap;
    private Vector3Int _playerPositionOffset = Vector3Int.zero;
    private Vector2[] _currentOctaveOffsets;
    private float _time = 0.01f;
    
    Vector2Int[] _currentNoiseGridTiles;

    private void OnEnable()
    {
        Character.OnPlayerWorldPositionUpdate += HandlePlayerPositionUpdate;
    }

    private void OnDisable()
    {
        Character.OnPlayerWorldPositionUpdate -= HandlePlayerPositionUpdate;
    }

    private void Awake()
    {
        _windRenderer = GetComponent<NoiseRenderer>();
        _noiseMap = new float[_noiseSettings.mapWidth, _noiseSettings.mapHeight];
        _currentNoiseGridTiles = new Vector2Int[_noiseSettings.mapWidth * _noiseSettings.mapHeight];
    }

    public override void Initialize(WeatherGrid grid)
    {
        base.Initialize(grid);

        _currentOctaveOffsets = new Vector2[_noiseSettings.octaves];
        _currentOctaveOffsets = NoiseGenerator.GenerateOctaveOffsets(_noiseSettings.octaves);

        EnableSystem();
    }

    private void Update()
    {
        if (!_isEnabled)
            return;

        _noiseMap = NoiseGenerator.GenerateNoiseMap(_noiseSettings, _currentOctaveOffsets, _playerPositionOffset, _time, _direction);
        _time += Time.deltaTime * _windSpeed;

        Vector2Int playerGridPos = _weatherGrid.WorldToGrid(_playerPositionOffset);

        for (int x = 0; x < _noiseSettings.mapWidth; x++)
        {
            for (int y = 0; y < _noiseSettings.mapHeight; y++)
            {
                _currentNoiseGridTiles[x + y * _noiseSettings.mapWidth] = new Vector2Int(x + playerGridPos.x, y + playerGridPos.y);
            }
        }

        _weatherGrid.SetWeatherSystem(_currentNoiseGridTiles, this);

        if (_debug)
            _windRenderer.RenderNoiseMapByColor(_noiseMap, _noiseSettings, _playerPositionOffset);
    }


    private void HandlePlayerPositionUpdate(Character playerCharacter)
    {
        if (!_isEnabled)
            return;
        _playerPositionOffset = (Vector3Int)_weatherGrid.WorldToGrid(playerCharacter.transform.position);
    }

    public override WeatherModifier GetWeatherModifier(Vector3 worldPos)
    {

        return new WeatherModifier();
    }
}
