using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

public class NoiseGenerator : MonoBehaviour
{
    [Header("Debug Settings")]
    // TODO : Replace with a call from the WindMap class
    [SerializeField] private int _debugNoiseMapWidth = 100;
    [SerializeField] private int _debugNoiseMapHeight = 100;
    [SerializeField] private int _debugOctaves = 1;
    [SerializeField] private Vector2 _debugNoiseScale = new Vector2(1f, 1f);

    [Tooltip("Controls decrease in amplitude of octaves. Further octaves have less influence on the final result." +
        "\nOn a map : Increasing persistance increases the influence of these small features")]
    [Range(0.0f, 1.0f)]
    [SerializeField] private float _debugPersistance = 0.5f;

    [Tooltip("Controls increase in frequency of octaves.\nOn a map : Increases the number of small features.")]
    [SerializeField] private float _debugLacunarity = 1;

    [SerializeField] private Tilemap _debugTileMap;
    [SerializeField] private TileBase _debugTile;
    [SerializeField] private Vector2 _debugDirection;

    private Vector3Int _playerPositionOffset = Vector3Int.zero;
    private Vector2[] _currentOctaveOffsets;
    private float _time = 0.01f;

    

    private void Start()
    {
        // TODO : Add seed? Do I need?
        // TODO : This octave thing isn't safe
        System.Random prng = new System.Random();

        _currentOctaveOffsets = new Vector2[_debugOctaves];

        for (int i = 0; i < _debugOctaves; i++)
        {
            float offsetX = prng.Next(-100000, 100000);
            float offsetY = prng.Next(-100000, 100000);
            _currentOctaveOffsets[i] = new Vector2(offsetX, offsetY);
        }
    }

    private void OnEnable()
    {
        Character.OnPlayerWorldPositionUpdate += HandlePlayerPositionUpdate;
    }

    private void OnDisable()
    {
        Character.OnPlayerWorldPositionUpdate -= HandlePlayerPositionUpdate;
    }

    private void Update()
    {
        GenerateDebugMap();
        _time += Time.deltaTime;
    }

    private Color GetColorByNoiseValue(float noiseValue)
    {
        Color newColor = Color.white;
        newColor.a = noiseValue;
        return newColor;
    }

    public void GenerateDebugMap()
    {
        _debugTileMap.ClearAllTiles();

        float[,] noiseMap = GenerateNoiseMap(_debugNoiseMapWidth, _debugNoiseMapHeight, _debugNoiseScale, _debugOctaves, _currentOctaveOffsets, _debugPersistance, _debugLacunarity);

        Vector3Int[] tilePositions = new Vector3Int[_debugNoiseMapWidth * _debugNoiseMapHeight];
        TileBase[] tiles = new TileBase[_debugNoiseMapHeight * _debugNoiseMapWidth];

        int mapHalfWidth = _debugNoiseMapWidth / 2;
        int mapHalfHeight = _debugNoiseMapHeight / 2;

        for (int x = 0; x < _debugNoiseMapWidth; x++)
        {
            for (int y = 0; y < _debugNoiseMapHeight; y++)
            {
                tilePositions[x + _debugNoiseMapWidth * y] = new Vector3Int(x - mapHalfWidth, y - mapHalfHeight) + _playerPositionOffset;
                tiles[x + _debugNoiseMapWidth * y] = _debugTile;
            }
        }

        _debugTileMap.SetTiles(tilePositions, tiles);

        for (int x = 0; x < _debugNoiseMapWidth; x++)
        {
            for (int y = 0; y < _debugNoiseMapHeight; y++)
            {
                _debugTileMap.SetTileFlags(new Vector3Int(x - mapHalfWidth, y - mapHalfHeight) + _playerPositionOffset, TileFlags.None);
                _debugTileMap.SetColor(new Vector3Int(x - mapHalfWidth, y - mapHalfHeight) + _playerPositionOffset, GetColorByNoiseValue(noiseMap[x,y]));
            }
        }
    }

    public float[,] GenerateNoiseMap(int mapWidth, int mapHeight, Vector2 noiseScale, int octaves, Vector2[] octaveOffsets, float persistance, float lacunarity)
    {
        float[,] noiseMap = new float[mapWidth, mapHeight];
        // Prevent division by 0
        if (noiseScale.x <= 0)
            noiseScale.x = 0.0001f;

        if (noiseScale.y <= 0)
            noiseScale.y = 0.0001f;

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        float halfWidth = mapWidth / 2f;
        float halfHeight = mapHeight / 2f;

        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                for (int i = 0; i < octaves; i++)
                {
                    float sampleX = (x - halfWidth + _playerPositionOffset.x) / noiseScale.x * frequency + octaveOffsets[i].x + -_time * _debugDirection.normalized.x;
                    float sampleY = (y - halfHeight + _playerPositionOffset.y) / noiseScale.y * frequency + octaveOffsets[i].y + -_time * _debugDirection.normalized.y;

                    // * 2 - 1  --> Perlin only returns 0 to 1. * 2 - 1 allows to get values from -1 to +1
                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;

                    // amplitude decreases each octave, since persistance should be less than 1
                    amplitude *= persistance;
                    // Frequency increases each octave, since lacunarity should be greater than 1
                    frequency *= lacunarity;

                    if (noiseHeight > maxNoiseHeight)
                        maxNoiseHeight = noiseHeight;
                    else if (noiseHeight < minNoiseHeight)
                        minNoiseHeight = noiseHeight;

                    noiseMap[x, y] = noiseHeight;
                }
            }
        }

        // Normalize noise map back to 0-1
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                // Inverse Lerp : Takes a value, and lerps between a min-max to return an alpha value (0-1)
                noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
            }
        }
        return noiseMap;
    }


    private void HandlePlayerPositionUpdate(Character playerCharacter)
    {
        Vector3 playerPos = playerCharacter.transform.position;
        Vector3Int playerPosInt = new Vector3Int();
        playerPosInt.x = Mathf.RoundToInt(playerPos.x);
        playerPosInt.y = Mathf.RoundToInt(playerPos.y);
        playerPosInt.z = 0;

        _playerPositionOffset = playerPosInt;
    }
}


