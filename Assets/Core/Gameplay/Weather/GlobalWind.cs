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
    }

    public override void Initialize(LegacyWeatherGrid grid)
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

        Vector3Int[] GridPositions = new Vector3Int[_noiseSettings.mapWidth * _noiseSettings.mapHeight];

        _time += Time.deltaTime * _windSpeed;

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

        _playerPositionOffset = _weatherGrid.WorldToGridPos(playerCharacter.transform.position);
    }
}
