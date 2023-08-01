using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;

[RequireComponent(typeof(Grid))]
public abstract class GridBase<T> : MonoBehaviour
{
    public Action<List<ChunkData<T>>> OnChunksUpdate;

    public TextMeshProUGUI playerChunkText;

    [Header("Grid Settings")]
    [SerializeField] protected int _chunkSizeX = 10;
    [SerializeField] protected int _chunkSizeY = 10;

    [SerializeField] protected int _chunkRenderDistance;

    // Components
    protected Grid _grid;
    protected Character _playerCharacter;

    // Grid
    protected float _gridCellSizeX;
    protected float _gridCellSizeY;

    protected Dictionary<Vector2Int, ChunkData<T>> _allChunks = new Dictionary<Vector2Int, ChunkData<T>>();
    protected Vector2Int _currentPlayerChunk;
    protected T _currentPlayerTile;
    protected List<ChunkData<T>> _loadedChunks = new List<ChunkData<T>>();

    protected List<IGridActor<T>> _registeredGridActors = new List<IGridActor<T>>();
    protected int _registeredActorsCount;

    protected virtual void OnEnable()
    {
        Character.OnPlayerWorldPositionUpdate += HandleNewPlayerPositionUpdate;
        Character.OnPlayerCharacterEnabled += HandleNewPlayerCharacter;
    }

    protected virtual void OnDisable()
    {
        Character.OnPlayerWorldPositionUpdate -= HandleNewPlayerPositionUpdate;
        Character.OnPlayerCharacterEnabled -= HandleNewPlayerCharacter;
    }

    protected void HandleNewPlayerCharacter(Character player)
    {
        this._playerCharacter = player;
    }

    public void RegisterGridActor(IGridActor<T> newGridActor)
    {
        _registeredGridActors.Add(newGridActor);
        _registeredActorsCount++;
    }

    public void UnregisterGridActor(IGridActor<T> newGridActor)
    {
        _registeredGridActors.Remove(newGridActor);
        _registeredActorsCount--;
    }

    protected virtual void Start()
    {
    }

    protected virtual void Update()
    {
        if (_playerCharacter != null)
            _currentPlayerTile = WorldToTile(_playerCharacter.transform.position);

        for (int i = 0; i < _registeredActorsCount; i++)
        {
            Vector3 worldPos = _registeredGridActors[i].GetWorldPosition();
            T tileData = WorldToTile(worldPos);
            _registeredGridActors[i].AssignCurrentTileData(tileData);
        }
    }

    protected virtual void Awake()
    {
        _grid = GetComponent<Grid>();
        _gridCellSizeX = _grid.cellSize.x;
        _gridCellSizeY = _grid.cellSize.y;
    }

    protected virtual void UpdateChunks()
    {
        bool update = false;

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
                {
                    LoadChunk(chunkData);
                    update = true;
                }
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

        int count = chunksToRemove.Count;
        if (count > 0)
            update = true;

        for (int i = 0; i < count; i++)
        {
            UnloadChunk(chunksToRemove[i]);
        }

        if (update)
            if (OnChunksUpdate != null)
                OnChunksUpdate(_loadedChunks);
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
        if (playerChunkText != null)
            playerChunkText.text = "Player in chunk : " + _currentPlayerChunk.ToString();
    }

    public virtual Vector2Int WorldToChunk(Vector3 worldPos)
    {
        Vector2Int ChunkPos = Vector2Int.zero;
        ChunkPos.x = Mathf.FloorToInt(worldPos.x / (_chunkSizeX * _gridCellSizeX));
        ChunkPos.y = Mathf.FloorToInt(worldPos.y / (_chunkSizeY * _gridCellSizeY));

        return ChunkPos;
    }

    public virtual Vector2Int WorldToGrid(Vector3 worldPos)
    {
        Vector2Int gridPos = new Vector2Int();
        gridPos.x = Mathf.FloorToInt(worldPos.x / _gridCellSizeX);
        gridPos.y = Mathf.FloorToInt(worldPos.y / _gridCellSizeY);
        return gridPos;
    }

    public virtual T GridToTile(int tileX, int tileY)
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
            return default(T);

        return targetChunk.GetTileData(tileX, tileY);
    }

    public virtual T WorldToTile(Vector3 worldPos)
    {
        throw new System.NotImplementedException();
    }

    protected abstract T[,] CreateTileData(int chunkX, int chunkY);
    protected abstract void LoadChunk(ChunkData<T> chunkData);
    protected abstract void UnloadChunk(ChunkData<T> chunkData);
}