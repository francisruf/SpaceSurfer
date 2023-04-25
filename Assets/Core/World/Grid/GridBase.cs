using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;

[RequireComponent(typeof(Grid))]
public abstract class GridBase<T> : MonoBehaviour
{
    public TextMeshProUGUI playerChunkText;

    [Header("Grid Settings")]
    [SerializeField] protected int _chunkSizeX = 10;
    [SerializeField] protected int _chunkSizeY = 10;

    [SerializeField] protected int _chunkRenderDistance;

    // Components
    protected Grid _grid;

    // Grid
    protected float _gridCellSizeX;
    protected float _gridCellSizeY;

    protected Dictionary<Vector2Int, ChunkData<T>> _allChunks = new Dictionary<Vector2Int, ChunkData<T>>();
    protected Vector2Int _currentPlayerChunk;
    protected List<ChunkData<T>> _loadedChunks = new List<ChunkData<T>>();


    protected virtual void OnEnable()
    {
        Character.OnPlayerWorldPositionUpdate += HandleNewPlayerPositionUpdate;
    }

    protected virtual void OnDisable()
    {
        Character.OnPlayerWorldPositionUpdate -= HandleNewPlayerPositionUpdate;
    }

    protected virtual void Awake()
    {
        _grid = GetComponent<Grid>();
        _gridCellSizeX = _grid.cellSize.x;
        _gridCellSizeY = _grid.cellSize.y;
    }

    protected virtual void UpdateChunks()
    {
        for (int x = _currentPlayerChunk.x - _chunkRenderDistance; x <= _currentPlayerChunk.x + _chunkRenderDistance; x++)
        {
            for (int y = _currentPlayerChunk.y - _chunkRenderDistance; y <= _currentPlayerChunk.y + _chunkRenderDistance; y++)
            {
                ChunkData<T> chunkData = GetChunkAtPosition(new Vector2Int(x, y));
                if (chunkData == null)
                {
                    T[,] tileData = CreateTileData(x, y);
                    chunkData = SpawnChunk(x, y, tileData);
                }

                if (chunkData == null)
                    continue;

                // TODO : Optimize
                if (!_loadedChunks.Contains(chunkData))
                    LoadChunk(chunkData);
            }
        }

        List<ChunkData<T>> chunksToRemove = new List<ChunkData<T>>();

        // TODO : Optimize
        foreach (var chunk in _loadedChunks)
        {
            if (chunk.chunkX < _currentPlayerChunk.x - _chunkRenderDistance)
            {
                chunksToRemove.Add(chunk);
                continue;
            }
            if (chunk.chunkY < _currentPlayerChunk.y - _chunkRenderDistance)
            {
                chunksToRemove.Add(chunk);
                continue;
            }
            if (chunk.chunkX > _currentPlayerChunk.x + _chunkRenderDistance)
            {
                chunksToRemove.Add(chunk);
                continue;
            }
            if (chunk.chunkY > _currentPlayerChunk.y + _chunkRenderDistance)
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

    protected virtual ChunkData<T> SpawnChunk(int chunkX, int chunkY, T[,] tileData)
    {
        ChunkData<T> newChunkData = new ChunkData<T>(chunkX, chunkY, tileData);
        _allChunks.Add(new Vector2Int(chunkX, chunkY), newChunkData);
        return newChunkData;
    }

    protected virtual ChunkData<T> SpawnChunk(Vector2Int chunkCoords, T[,] tileData)
    {
        return SpawnChunk(chunkCoords.x, chunkCoords.y, tileData);
    }

    protected virtual Vector2Int WorldToChunk(Vector3 worldPos)
    {
        Vector2Int ChunkPos = Vector2Int.zero;
        ChunkPos.x = Mathf.FloorToInt(worldPos.x / (_chunkSizeX * _gridCellSizeX));
        ChunkPos.y = Mathf.FloorToInt(worldPos.y / (_chunkSizeY * _gridCellSizeY));

        return ChunkPos;
    }

    protected virtual ChunkData<T> GetChunkAtPosition(Vector2Int chunkCoords)
    {
        ChunkData<T> chunk;
        _allChunks.TryGetValue(chunkCoords, out chunk);

        return chunk;
    }

    protected virtual void SetChunkDataAtPosition(Vector2Int chunkCoords, ChunkData<T> chunkData)
    {
        _allChunks.Add(chunkCoords, chunkData);
    }

    protected virtual void SetTileData(STileData<T>[] tileData)
    {
        foreach (var data in tileData)
        {
            SetTileData(data);
        }
    }

    protected virtual void SetTileData(STileData<T> tileData)
    {
        SetTileData(tileData.tileX, tileData.tileY, tileData.tileData);
    }

    protected virtual void SetTileData(int tileX, int tileY, T tileData)
    {
        Vector2Int chunkCoords = new Vector2Int();

        chunkCoords.x = Mathf.FloorToInt(tileX / (float)_chunkSizeX);
        chunkCoords.y = Mathf.FloorToInt(tileY / (float)_chunkSizeY);

        int chunkTileX = tileX % _chunkSizeX;
        int chunkTileY = tileY % _chunkSizeY;

        if (chunkTileX < 0)
            chunkTileX = _chunkSizeX - 1 - Mathf.Abs(chunkTileX);

        if (chunkTileY < 0)
            chunkTileY = _chunkSizeY - 1 - Mathf.Abs(chunkTileY);

        ChunkData<T> targetChunk = GetChunkAtPosition(chunkCoords);
        if (targetChunk == null)
        {
            T[,] newTileData = CreateTileData(chunkCoords.x, chunkCoords.y);
            targetChunk = SpawnChunk(chunkCoords, newTileData);
        }

        targetChunk.SetTileData(chunkTileX, chunkTileY, tileData);
    }

    protected virtual void HandleNewPlayerPositionUpdate(Character character)
    {
        _currentPlayerChunk = WorldToChunk(character.transform.position);
        UpdateChunks();
        playerChunkText.text = "Player in chunk : " + _currentPlayerChunk.ToString();
    }

    protected abstract T[,] CreateTileData(int chunkX, int chunkY);
    protected abstract void LoadChunk(ChunkData<T> chunkData);
    protected abstract void UnloadChunk(ChunkData<T> chunkData);
}