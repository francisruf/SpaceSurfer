using System.Collections.Generic;
using UnityEngine;

public class Wind_GlobalClouds : Wind_Base
{
    // Components
    private NoiseRenderer _windRenderer;

    [Header("Settings")]
    [SerializeField] private NoiseSettings _noiseSettings;
    [SerializeField] private Vector2 _direction;
    [SerializeField] private float _scrollSpeed = 1f;
    [SerializeField] private AnimationCurve _noiseToWindCurve;

    [Header("Debug")]
    [SerializeField] private NoiseRenderMode _noiseRenderMode;

    // Private variables
    private float[,] _noiseMap;
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
        _noiseMap = new float[_noiseSettings.mapWidth, _noiseSettings.mapHeight];
    }

    public override void Initialize(WindGrid grid)
    {
        base.Initialize(grid);
        _currentOctaveOffsets = new Vector2[_noiseSettings.octaves];
        _currentOctaveOffsets = NoiseGenerator.GenerateOctaveOffsets(_noiseSettings.octaves);
    }

    public override List<WindForce> GenerateWindUpdate()
    {
        _windForces.Clear();
        if (!_isEnabled)
            return _windForces;

        _noiseMap = NoiseGenerator.GenerateNoiseMap(_noiseSettings, _currentOctaveOffsets, _playerPositionOffset, _time, _direction, true);
        Vector2 dir = _direction.normalized;

        for (int x = 0; x < _noiseSettings.mapWidth; x++)
        {
            for (int y = 0; y < _noiseSettings.mapHeight; y++)
            {
                Vector2Int cellCoords = new Vector2Int(x - _noiseSettings.mapWidth / 2 + _playerPositionOffset.x, y - _noiseSettings.mapHeight / 2 + _playerPositionOffset.y);
                _windForces.Add(new WindForce(cellCoords, _noiseToWindCurve.Evaluate(_noiseMap[x, y]), dir));
                //_currentNoiseGridTiles[x + y * _noiseSettings.mapWidth] = new Vector2Int(x + playerGridPos.x, y + playerGridPos.y);
            }
        }

        if (_debug)
            _windRenderer.RenderNoiseMapByColor(_noiseMap, _noiseSettings, _playerPositionOffset, _noiseRenderMode);

        return _windForces;
    }

    private void Update()
    {
        _time += Time.deltaTime * _scrollSpeed;
    }

    private void HandlePlayerPositionUpdate(Character playerCharacter)
    {
        if (!_isEnabled)
            return;
        _playerPositionOffset = _windGrid.WorldToCell(playerCharacter.transform.position);
    }

}
