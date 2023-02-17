// TEMP for testing. TODO : Delete this class and integrate into chunks

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;

public class GlobalObstacleController : MonoBehaviour
{
    // Components
    [SerializeField] private ObstacleNoiseRenderer _noiseRenderer;

    [SerializeField] private NoiseSettings _noiseSettings;
    [SerializeField] private SBlockingObstacleTiles[] _tiles;
    public SBlockingObstacleTiles[] Tiles { get { return _tiles; } }

    private Vector2[] _currentOctaveOffsets;

    private void Start()
    {
        _currentOctaveOffsets = new Vector2[_noiseSettings.octaves];
        _currentOctaveOffsets = NoiseGenerator.GenerateOctaveOffsets(_noiseSettings.octaves);
    }


    public TileBase[,] GenerateObstaclesInChunk(int chunkSize, int chunkX, int chunkY)
    {
        NoiseSettings settings = (NoiseSettings)ScriptableObject.CreateInstance(typeof(NoiseSettings));
        settings.mapWidth = chunkSize;
        settings.mapHeight = chunkSize;
        settings.octaves = _noiseSettings.octaves;
        settings.noiseScale = _noiseSettings.noiseScale;
        settings.persistance = _noiseSettings.persistance;
        settings.lacunarity = _noiseSettings.lacunarity;

        Vector3Int gridOffset = new Vector3Int(chunkX * chunkSize, chunkY * chunkSize, 0);
        float[,] noiseMap = NoiseGenerator.GenerateNoiseMap(settings, _currentOctaveOffsets, gridOffset, 0f, Vector2.zero, false);

        TileBase[,] tiles = new TileBase[chunkSize, chunkSize];

        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
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
        _currentOctaveOffsets = new Vector2[_noiseSettings.octaves];
        _currentOctaveOffsets = NoiseGenerator.GenerateOctaveOffsets(_noiseSettings.octaves);

        float[,] noiseMap = NoiseGenerator.GenerateNoiseMap(_noiseSettings, _currentOctaveOffsets, Vector3Int.zero, 0f, Vector2.zero, false);
        _noiseRenderer.RenderNoiseMapByTile(noiseMap, _noiseSettings, Vector3Int.zero);
    }
}

[CustomEditor(typeof(GlobalObstacleController))]
public class GlobalObstacleControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        GlobalObstacleController controller = (GlobalObstacleController)target;
        if (DrawDefaultInspector())
        {
        }

        if (GUILayout.Button("Generate test"))
            controller.GenerateDebugMap();
    }
}

[System.Serializable]
public struct SBlockingObstacleTiles
{
    public TileBase tile;
    public float value;
}
