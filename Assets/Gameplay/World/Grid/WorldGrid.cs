using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldGrid : GridBase<WorldTileData>
{
    [Header("World Settings")]
    [SerializeField] private WorldType _worldType;
    [SerializeField] private TileBase[] _tilePool;
    [SerializeField] private Tilemap _worldTilemap;

    [Header("Debug")]
    [SerializeField] private TileBase _debugTile;

    private GlobalObstacleController _obstacleController;

    protected override void Awake()
    {
        base.Awake();
        _obstacleController = FindObjectOfType<GlobalObstacleController>();
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override WorldTileData[,] CreateTileData(int chunkX, int chunkY)
    {
        WorldTileData[,] tileData = new WorldTileData[_chunkSizeX, _chunkSizeY];
        TileBase chunkTile = null;

        switch (_worldType)
        {

            case WorldType.ProceduralEmpty:

                chunkTile = _tilePool[Random.Range(0, _tilePool.Length)];
                for (int x = 0; x < _chunkSizeX; x++)
                {
                    for (int y = 0; y < _chunkSizeY; y++)
                    {
                        WorldTileData data = new WorldTileData(chunkTile);
                        tileData[x, y] = data;
                    }
                }

                break;

            case WorldType.ProceduralObstacles:
                
                TileBase[,] obstacleTiles = _obstacleController.GenerateObstaclesInChunk(_chunkSizeX, _chunkSizeY, chunkX, chunkY);

                // TODO : Temporary random tile assigned on spawn
                chunkTile = _tilePool[Random.Range(0, _tilePool.Length)];

                for (int x = 0; x < _chunkSizeX; x++)
                {
                    for (int y = 0; y < _chunkSizeY; y++)
                    {
                        WorldTileData data = new WorldTileData(chunkTile);
                        tileData[x, y] = data;

                        if (obstacleTiles[x, y] != null)
                            tileData[x,y].worldTile = obstacleTiles[x, y];
                    }
                }
                break;
        }
        return tileData;
    }

    protected override void LoadChunk(ChunkData<WorldTileData> chunkData)
    {
        Vector3Int[] tilePositions = new Vector3Int[_chunkSizeX * _chunkSizeY];
        TileBase[] tiles = new TileBase[_chunkSizeX * _chunkSizeY];

        int i = 0;
        for (int x = 0; x < _chunkSizeX; x++)
        {
            for (int y = 0; y < _chunkSizeY; y++)
            {
                tilePositions[i].x = chunkData.chunkX * _chunkSizeX + x;
                tilePositions[i].y = chunkData.chunkY * _chunkSizeY + y;
                tilePositions[i].z = 0;

                tiles[i] = chunkData.tileData[x, y].worldTile;

                i++;
            }
        }
        _worldTilemap.SetTiles(tilePositions, tiles);
        _loadedChunks.Add(chunkData);
    }

    protected override void UnloadChunk(ChunkData<WorldTileData> chunkData)
    {
        if (chunkData == null)
            return;

        for (int x = 0; x < _chunkSizeX; x++)
        {
            for (int y = 0; y < _chunkSizeY; y++)
            {
                int targetTileX = chunkData.chunkX * _chunkSizeX + x;
                int targetTileY = chunkData.chunkY * _chunkSizeY + y;

                _worldTilemap.SetTile(new Vector3Int(targetTileX, targetTileY), null);

            }
        }
        _loadedChunks.Remove(chunkData);
    }

    private IEnumerator TestTileSpawning()
    {
        bool execute = true;

        while (execute)
        {
            yield return new WaitForSeconds(0.1f);

            STileData<WorldTileData>[] newTileData = new STileData<WorldTileData>[20*20];

            for (int x = 0; x < 20; x++)
            {
                for (int y = 0; y < 20; y++)
                {
                    STileData<WorldTileData> data = new STileData<WorldTileData>();
                    data.tileX = x;
                    data.tileY = y;
                    data.tileData = new WorldTileData(_debugTile);

                    newTileData[x + y * 20] = data;
                }
            }

            SetTileData(newTileData);
            //Debug.Log("Assigning data at : (" + x + "," + y + ")");
            //SetTileData(x, y, new WorldTileData(_debugTile));

            execute = false;
        }
    }
}
