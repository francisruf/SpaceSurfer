using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class LegacyWeatherGrid : MonoBehaviour
{
    [SerializeField] private Grid _tilemapGrid;
    public Grid TilemapGrid { get { return _tilemapGrid; } }

    public float gridCellSize;
    // TODO : Remove hardcoded value and do something better lul

    private int gridSize = 500;
    private WeatherTile[,] _weatherTiles;

    private void Awake()
    {
        _weatherTiles = new WeatherTile[gridSize, gridSize];
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
                _weatherTiles[x, y] = new WeatherTile();
        }
    }

    private void Update()
    {

    }

    private void OnEnable()
    {
        Character.OnPlayerWorldPositionUpdate += HandlePlayerPositionUpdate;
    }

    private void OnDisable()
    {
        Character.OnPlayerWorldPositionUpdate -= HandlePlayerPositionUpdate;
    }

    public void AssignWeatherSystem(Vector3Int[] gridPositions, WeatherSystem weatherSystem)
    {
        foreach (var gridPos in gridPositions)
        {
            GetWeatherTileAtGridPos(gridPos).AddWeatherSystems(weatherSystem);
        }
    }

    public Vector3Int WorldToGridPos(Vector3 worldPos)
    {
        Vector3Int gridPos = new Vector3Int();
        gridPos.x = Mathf.RoundToInt(worldPos.x / gridCellSize);
        gridPos.y = Mathf.RoundToInt(worldPos.y / gridCellSize);

        return gridPos;
    }

    public Vector3 GridToWorldPos(Vector3Int gridPos)
    {
        Vector3 worldPos = new Vector3();
        worldPos.x = gridPos.x * gridCellSize;
        worldPos.y = gridPos.y * gridCellSize;

        return worldPos;
    }

    public WeatherTile GetWeatherTileAtGridPos(Vector3Int gridPos)
    {
        return _weatherTiles[gridPos.x + gridSize / 2, gridPos.y + gridPos.y / 2];
    }

    private void HandlePlayerPositionUpdate(Character player)
    {
        Vector3Int tileGridPos = WorldToGridPos(player.transform.position);
        Vector3 tileWorldPos = GridToWorldPos(tileGridPos);

        Debug.Log(tileGridPos);

        if (GetWeatherTileAtGridPos(tileGridPos).HasWeatherSystem)
            Debug.DrawLine(tileWorldPos, tileWorldPos + (Vector3)(Vector2.up * 0.25f), Color.green, 1f);
        else
            Debug.DrawLine(tileWorldPos, tileWorldPos + (Vector3)(Vector2.up * 0.25f), Color.red, 1f);
    }
}