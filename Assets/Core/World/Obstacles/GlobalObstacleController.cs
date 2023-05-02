// TEMP for testing. TODO : Delete this class and integrate into chunks

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;

public class GlobalObstacleController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private NoiseSettings _noiseSettings;

    [SerializeField] private SNoiseTile[] _tiles;
    public SNoiseTile[] Tiles { get { return _tiles; } }

    [Header("Debug")]
    [SerializeField] private NoiseRenderer _debugObstacleRenderer;

    private Vector2[] _currentOctaveOffsets;


    private void GenerateOctaves()
    {
        _currentOctaveOffsets = new Vector2[_noiseSettings.octaves];
        _currentOctaveOffsets = NoiseGenerator.GenerateOctaveOffsets(_noiseSettings.octaves);
    }

    public TileBase[,] GenerateObstaclesInChunk(int chunkSizeX, int chunkSizeY, int chunkX, int chunkY)
    {
        if (_currentOctaveOffsets == null)
            GenerateOctaves();

        NoiseSettings settings = (NoiseSettings)ScriptableObject.CreateInstance(typeof(NoiseSettings));
        settings.mapWidth = chunkSizeX;
        settings.mapHeight = chunkSizeY;
        settings.octaves = _noiseSettings.octaves;
        settings.noiseScale = _noiseSettings.noiseScale;
        settings.persistance = _noiseSettings.persistance;
        settings.lacunarity = _noiseSettings.lacunarity;

        Vector3Int gridOffset = new Vector3Int(chunkX * chunkSizeX, chunkY * chunkSizeY, 0);

        float[,] noiseMap = NoiseGenerator.GenerateNoiseMap(settings, _currentOctaveOffsets, gridOffset, 0f, Vector2.zero, false);
        TileBase[,] tiles = new TileBase[chunkSizeX, chunkSizeY];

        for (int x = 0; x < chunkSizeX; x++)
        {
            for (int y = 0; y < chunkSizeY; y++)
            {
                tiles[x, y] = SelectTileByNoise(noiseMap[x,y]);
            }
        }

        return tiles;
    }
    private TileBase SelectTileByNoise(float noiseValue)
    {
        int length = _tiles.Length;

        if (length == 0)
            return null;

        for (int i = 0; i < length; i++)
        {
            if (_tiles[i].value >= noiseValue)
                return _tiles[i].tile;
        }
        return null;
    }

    public void GenerateDebugMap()
    {
        if (_debugObstacleRenderer == null)
        {
            Debug.LogError("No noise renderer assigned to GlobalObstacleController.");
            return;
        }

        _currentOctaveOffsets = new Vector2[_noiseSettings.octaves];
        _currentOctaveOffsets = NoiseGenerator.GenerateOctaveOffsets(_noiseSettings.octaves);

        float[,] noiseMap = NoiseGenerator.GenerateNoiseMap(_noiseSettings, _currentOctaveOffsets, Vector3Int.zero, 0f, Vector2.zero, false);
        _debugObstacleRenderer.RenderNoiseMapByTile(noiseMap, _noiseSettings, Vector3Int.zero, _tiles);
    }

    public void CleanUpDebugMap()
    {
        _debugObstacleRenderer.ClearAllTiles();
    }
}



[System.Serializable]
public struct SNoiseTile
{
    public TileBase tile;
    public float value;
}
