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

        float[,] noiseMap = NoiseGenerator.GenerateNoiseMap(_noiseSettings, _currentOctaveOffsets, _playerPositionOffset, _time, _direction, false);
        _time += Time.deltaTime * _windSpeed;

        Vector2Int playerGridPos = _weatherGrid.WorldToGrid(_playerPositionOffset);
        int xMin = playerGridPos.x - _noiseSettings.mapWidth / 2;
        int xMax = playerGridPos.x + _noiseSettings.mapWidth / 2;
        int yMin = playerGridPos.y - _noiseSettings.mapWidth / 2;
        int yMax = playerGridPos.y + _noiseSettings.mapWidth / 2;

        //for (int x = xMin; x < xMax; x++)
        //{
        //    for (int y = yMin; y < yMax; y++)
        //    {
        //        _currentNoiseGridTiles[x + y * _noiseSettings.mapWidth + y] = new Vector2Int(x, y);
        //    }
        //}

        //_weatherGrid.SetWeatherSystem(_currentNoiseGridTiles, this);

        if (_debug)
            _windRenderer.RenderNoiseMapByColor(noiseMap, _noiseSettings, _playerPositionOffset);
    }

    private void HandlePlayerPositionUpdate(Character playerCharacter)
    {
        if (!_isEnabled)
            return;

        //Vector3 playerPos = playerCharacter.transform.position;
        //Vector3Int playerPosInt = new Vector3Int();
        //playerPosInt.x = Mathf.RoundToInt(playerPos.x / _weatherGrid.gridSize);
        //playerPosInt.y = Mathf.RoundToInt(playerPos.y / _weatherGrid.gridSize);
        //playerPosInt.z = 0;

        _playerPositionOffset = (Vector3Int)_weatherGrid.WorldToGrid(playerCharacter.transform.position);
    }
}
