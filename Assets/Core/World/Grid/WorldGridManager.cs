using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Tilemaps;

public class WorldGridManager : MonoBehaviour
{
    // Public params
    public WorldSettings worldSettings;
    [SerializeField] private TileBase[] tilePool;

    // Components
    private Grid _grid;
    private Tilemap _tilemap;
    private GlobalObstacleController _obstacleController;

    // World data
    // TODO : This needs to be nested in an even bigger coordinate system
    private int _maxWorldSize = 500;
    private ChunkData[,] _allChunkData;
    private List<ChunkData> _renderedChunks = new List<ChunkData>();
    private Vector3Int currentPlayerChunk;

    private Vector3Int _originTile;

    private void OnEnable()
    {
        Character.OnPlayerWorldPositionUpdate += HandleNewPlayerPositionUpdate;
    }

    private void OnDisable()
    {
        Character.OnPlayerWorldPositionUpdate -= HandleNewPlayerPositionUpdate;
    }

    private void Awake()
    {
        _obstacleController = FindObjectOfType<GlobalObstacleController>();
        _grid = GetComponent<Grid>();
        _tilemap = GetComponentInChildren<Tilemap>();
        _allChunkData = new ChunkData[_maxWorldSize, _maxWorldSize];

        _grid.cellSize = new Vector3(worldSettings.gridCellSize, worldSettings.gridCellSize);
    }

    private Vector3Int WorldToChunk(Vector3 worldPos)
    {
        Vector3Int ChunkPos = Vector3Int.zero;
        ChunkPos.x = Mathf.FloorToInt(worldPos.x / (worldSettings.chunkSize * worldSettings.gridCellSize));
        ChunkPos.y = Mathf.FloorToInt(worldPos.y / (worldSettings.chunkSize * worldSettings.gridCellSize));

        return ChunkPos;
    }

    private ChunkData GetChunkAtPosition(int chunkX, int chunkY)
    {
        int halfWorldSize = _maxWorldSize / 2;
        return _allChunkData[chunkX + halfWorldSize, chunkY + halfWorldSize];
    }

    private void SetChunkDataAtPosition(int chunkX, int chunkY, ChunkData newData)
    {
        int halfWorldSize = _maxWorldSize / 2;
        _allChunkData[chunkX + halfWorldSize, chunkY + halfWorldSize] = newData;
    }

    private void HandleNewPlayerPositionUpdate(Character character)
    {
        currentPlayerChunk = WorldToChunk(character.transform.position);
        //playerChunkText.text = "Player in chunk : " + currentPlayerChunk.ToString();

        UpdateChunks();
    }

 

    private void UpdateChunks()
    {
        if (worldSettings.worldType == WorldType.Handmade)
            return;

        int renderDist = worldSettings.chunkRenderDistance;

        for (int x = currentPlayerChunk.x - renderDist; x <= currentPlayerChunk.x + renderDist; x++)
        {
            for (int y = currentPlayerChunk.y - renderDist; y <= currentPlayerChunk.y + renderDist; y++)
            {
                ChunkData chunkData = GetChunkAtPosition(x, y);
                if (chunkData == null)
                {
                    switch (worldSettings.worldType)
                    {
                        case WorldType.ProceduralEmpty:
                            chunkData = SpawnEmptyChunk(x, y);
                            break;
                        case WorldType.ProceduralObstacles:
                            chunkData = SpawnChunkWithObstacleMap(x, y);
                            break;
                    }
                }

                if (chunkData == null)
                    continue;

                // TODO : Optimize
                if (!_renderedChunks.Contains(chunkData))
                    RenderChunk(chunkData);
            }
        }

        List<ChunkData> chunksToRemove = new List<ChunkData>();

        // TODO : Optimize
        foreach (var chunk in _renderedChunks)
        {
            if (chunk.chunkX < currentPlayerChunk.x - renderDist)
            {
                chunksToRemove.Add(chunk);
                continue;
            }
            if (chunk.chunkY < currentPlayerChunk.y - renderDist)
            {
                chunksToRemove.Add(chunk);
                continue;
            }
            if (chunk.chunkX > currentPlayerChunk.x + renderDist)
            {
                chunksToRemove.Add(chunk);
                continue;
            }
            if (chunk.chunkY > currentPlayerChunk.y + renderDist)
            {
                chunksToRemove.Add(chunk);
                continue;
            }
        }

        foreach (var chunk in chunksToRemove)
        {
            UnloadChunk(chunk);
        }
    }

    private ChunkData SpawnEmptyChunk(int chunkX, int chunkY)
    {
        if (chunkX >= 250 || chunkX <= -250 || chunkY >= 250 || chunkY <= -250)
        {
            Debug.LogError("Gridmanager : Attempted to spawn a chunk outside of maximum world size");
            return null;
        }

        int size = worldSettings.chunkSize;
        TileBase[,] newTiles = new TileBase[worldSettings.chunkSize, worldSettings.chunkSize];

        // TODO : Temporary random tile assigned on spawn
        TileBase chunkTile = tilePool[Random.Range(0, tilePool.Length)];

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                newTiles[x, y] = chunkTile;
            }
        }
        ChunkData newChunkData = new ChunkData(chunkX, chunkY, newTiles);
        SetChunkDataAtPosition(chunkX, chunkY, newChunkData);

        return newChunkData;
    }

    private ChunkData SpawnChunkWithObstacleMap(int chunkX, int chunkY)
    {
        if (_obstacleController == null)
        {
            Debug.LogError("GridManager could not find obstacle controller.");
            return SpawnEmptyChunk(chunkX, chunkY);
        }

        if (chunkX >= 250 || chunkX <= -250 || chunkY >= 250 || chunkY <= -250)
        {
            Debug.LogError("Gridmanager : Attempted to spawn a chunk outside of maximum world size");
            return null;
        }

        int size = worldSettings.chunkSize;
        TileBase[,] newTiles = new TileBase[worldSettings.chunkSize, worldSettings.chunkSize];
        TileBase[,] obstacles = _obstacleController.GenerateObstaclesInChunk(size, chunkX, chunkY);

        // TODO : Temporary random tile assigned on spawn
        TileBase chunkTile = tilePool[Random.Range(0, tilePool.Length)];

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                if (obstacles[x, y] != null)
                {
                    newTiles[x, y] = obstacles[x, y];
                    continue;
                }
                newTiles[x, y] = chunkTile;
            }
        }
        ChunkData newChunkData = new ChunkData(chunkX, chunkY, newTiles);
        SetChunkDataAtPosition(chunkX, chunkY, newChunkData);

        return newChunkData;
    }

    private void RenderChunk(ChunkData chunkData)
    {
        int size = worldSettings.chunkSize;
        Vector3Int[] tilePositions = new Vector3Int[worldSettings.chunkSize * worldSettings.chunkSize];
        TileBase[] tiles = new TileBase[worldSettings.chunkSize * worldSettings.chunkSize];

        int i = 0;
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                tilePositions[i].x = chunkData.chunkX * size + x;
                tilePositions[i].y = chunkData.chunkY * size + y;
                tilePositions[i].z = 0;

                tiles[i] = chunkData.tiles[x, y];

                i++;
            }
        }
        _tilemap.SetTiles(tilePositions, tiles);
        _renderedChunks.Add(chunkData);
    }

    private void UnloadChunk(ChunkData chunkData)
    {
        if (chunkData == null)
            return;

        int size = worldSettings.chunkSize;

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                int targetTileX = chunkData.chunkX * size + x;
                int targetTileY = chunkData.chunkY * size + y;

                _tilemap.SetTile(new Vector3Int(targetTileX, targetTileY), null);

            }
        }
        _renderedChunks.Remove(chunkData);
    }

    private void UnloadChunk(int chunkX, int chunkY)
    {
        ChunkData chunkData = GetChunkAtPosition(chunkX, chunkY);
        UnloadChunk(chunkData);
    }



    void Update()
    {
        // Calculate when player enters a chunk
        // Spawn 2 chunks ahead (of X and Y)
        // Despawn 2 chunks behind

        // Most likely need to store this in a Chunk class with its own functionality

        // Spawn random object in chunk
    }
}