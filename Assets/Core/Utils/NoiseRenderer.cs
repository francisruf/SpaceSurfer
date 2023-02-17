using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class NoiseRenderer : MonoBehaviour
{
    [SerializeField] protected Tilemap _targetTileMap;
    [SerializeField] protected TileBase _defaultTile;
  
    public virtual void RenderNoiseMapByColor(float[,] noiseMap, NoiseSettings settings, Vector3Int globalOffset)
    {
        _targetTileMap.ClearAllTiles();

        int mapWidth = settings.mapWidth;
        int mapHeight = settings.mapHeight;

        Vector3Int[] tilePositions = new Vector3Int[mapWidth * mapHeight];
        TileBase[] tiles = new TileBase[mapHeight * mapWidth];

        int mapHalfWidth = mapWidth / 2;
        int mapHalfHeight = mapHeight / 2;

        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                tilePositions[x + mapWidth * y] = new Vector3Int(x - mapHalfWidth, y - mapHalfHeight) + globalOffset;
                tiles[x + mapWidth * y] = _defaultTile;
            }
        }

        _targetTileMap.SetTiles(tilePositions, tiles);

        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                _targetTileMap.SetTileFlags(new Vector3Int(x - mapHalfWidth, y - mapHalfHeight) + globalOffset, TileFlags.None);
                _targetTileMap.SetColor(new Vector3Int(x - mapHalfWidth, y - mapHalfHeight) + globalOffset, GetColorByNoiseValue(noiseMap[x, y]));
            }
        }
    }

    public virtual void RenderNoiseMapByTile(float[,] noiseMap, NoiseSettings settings, Vector3Int globalOffset)
    {
        _targetTileMap.ClearAllTiles();

        int mapWidth = settings.mapWidth;
        int mapHeight = settings.mapHeight;

        Vector3Int[] tilePositions = new Vector3Int[mapWidth * mapHeight];
        TileBase[] tiles = new TileBase[mapHeight * mapWidth];

        int mapHalfWidth = mapWidth / 2;
        int mapHalfHeight = mapHeight / 2;

        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                tilePositions[x + mapWidth * y] = new Vector3Int(x - mapHalfWidth, y - mapHalfHeight) + globalOffset;
                tiles[x + mapWidth * y] = SelectTileByNoise(noiseMap[x,y]);
            }
        }

        _targetTileMap.SetTiles(tilePositions, tiles);
    }

    protected virtual Color GetColorByNoiseValue(float noiseValue)
    {
        Color newColor = Color.white;
        newColor.a = noiseValue;
        return newColor;
    }

    protected virtual TileBase SelectTileByNoise(float noiseValue)
    {
        return _defaultTile;
    }
}
