using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{
    // DEBUG
    public TextMeshProUGUI playerChunkText;

    // Public params
    public WorldSettings worldSettings;
    [SerializeField] private TileBase _playerStartTile;
    [SerializeField] private TileBase[] tilePool;
    [SerializeField] private TileBase _defaultTile;
    [SerializeField] private Texture2D _textureMap;

    // Components
    private Grid _grid;
    private Tilemap _tilemap;

    // World data
    // TODO : This needs to be nested in an even bigger coordinate system
    private int _maxWorldSize = 500;
    private ChunkData[,] _allChunkData;
    private List<ChunkData> _renderedChunks = new List<ChunkData>();
    private Vector3Int currentPlayerChunk;

    private Vector3Int _originTile;

    private void OnEnable()
    {
        PlayerController.OnWorldPositionUpdate += HandleNewPlayerPositionUpdate;
    }

    private void OnDisable()
    {
        PlayerController.OnWorldPositionUpdate -= HandleNewPlayerPositionUpdate;
    }

    private void Awake()
    {
        _grid = GetComponent<Grid>();
        _tilemap = GetComponentInChildren<Tilemap>();
        _allChunkData = new ChunkData[_maxWorldSize, _maxWorldSize];

        _grid.cellSize = new Vector3(worldSettings.gridCellSize, worldSettings.gridCellSize);
    }

    private void Start()
    {
        PlayerController player = FindObjectOfType<PlayerController>();
        _originTile = _tilemap.WorldToCell(player.transform.position);

        //SpawnStartTiles();
        //_tilemap.SetTile(_originTile, _playerStartTile);
        //SpawnTilesFromTexture(_textureMap, 5f);
    }

    private Vector3Int WorldToChunk(Vector3 worldPos)
    {
        Vector3Int ChunkPos = Vector3Int.zero;
        ChunkPos.x = Mathf.FloorToInt(worldPos.x / (worldSettings.chunkSize * worldSettings.gridCellSize));
        ChunkPos.y = Mathf.FloorToInt(worldPos.y / (worldSettings.chunkSize * worldSettings.gridCellSize));

        return ChunkPos;
    }


    private void HandleNewPlayerPositionUpdate(PlayerController controller)
    {
        currentPlayerChunk = WorldToChunk(controller.transform.position);
        //playerChunkText.text = "Player in chunk : " + currentPlayerChunk.ToString();

        int renderDist = worldSettings.chunkRenderDistance;

        for (int x = currentPlayerChunk.x - renderDist; x <= currentPlayerChunk.x + renderDist; x++)
        {
            for (int y = currentPlayerChunk.y - renderDist; y <= currentPlayerChunk.y + renderDist; y++)
            {
                ChunkData chunkData = GetChunkAtPosition(x, y);
                if (chunkData == null)
                    chunkData = SpawnChunk(x, y);

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

    private void SpawnTilesFromTexture(Texture2D tex, float scaleFactor)
    {
        int texSizeX = tex.width;
        int texSizeY = tex.height;

        for (int x = 0; x < texSizeX / scaleFactor; x++)
        {
            for (int y = 0; y < texSizeY / scaleFactor; y++)
            {
                bool positivePixel = tex.GetPixel((int)(x * scaleFactor), (int)(y * scaleFactor)).r > 0f;

                Vector3Int tileCoord = new Vector3Int(x, y, 0);
                TileBase targetTile = positivePixel ? _playerStartTile : _defaultTile;
                _tilemap.SetTile(tileCoord, targetTile);
            }
        }
    }

    private void SpawnStartTiles()
    {
        int chunkAmount = worldSettings.chunkRenderDistance;

        for (int x = 0 - chunkAmount; x < 0 + chunkAmount; x++)
        {
            for (int y = 0 - chunkAmount; y < 0 + chunkAmount; y++)
            {
                SpawnChunk(x, y);
            }
        }
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

    private ChunkData SpawnChunk(int chunkX, int chunkY)
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
                newTiles[x,y] = chunkTile;
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