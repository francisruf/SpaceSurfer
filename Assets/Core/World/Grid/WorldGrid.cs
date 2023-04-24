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

    protected override WorldTileData[,] CreateTileData(int chunkX, int chunkY)
    {
        WorldTileData[,] tileData = new WorldTileData[_chunkSize, _chunkSize];
        TileBase chunkTile = null;

        switch (_worldType)
        {

            case WorldType.ProceduralEmpty:

                chunkTile = _tilePool[Random.Range(0, _tilePool.Length)];
                for (int x = 0; x < _chunkSize; x++)
                {
                    for (int y = 0; y < _chunkSize; y++)
                    {
                        WorldTileData data = new WorldTileData(chunkTile);
                        tileData[x, y] = data;
                    }
                }

                break;

            case WorldType.ProceduralObstacles:
                
                TileBase[,] obstacleTiles = _obstacleController.GenerateObstaclesInChunk(_chunkSize, chunkX, chunkY);

                // TODO : Temporary random tile assigned on spawn
                chunkTile = _tilePool[Random.Range(0, _tilePool.Length)];

                for (int x = 0; x < _chunkSize; x++)
                {
                    for (int y = 0; y < _chunkSize; y++)
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
        int size = _chunkSize;
        Vector3Int[] tilePositions = new Vector3Int[_chunkSize * _chunkSize];
        TileBase[] tiles = new TileBase[_chunkSize * _chunkSize];

        int i = 0;
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                tilePositions[i].x = chunkData.chunkX * size + x;
                tilePositions[i].y = chunkData.chunkY * size + y;
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

        int size = _chunkSize;

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                int targetTileX = chunkData.chunkX * size + x;
                int targetTileY = chunkData.chunkY * size + y;

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


            int x = Random.Range(-20, 20);
            int y = Random.Range(-20, 20);
            Debug.Log("Assigning data at : (" + x + "," + y + ")");

            SetTileData(x, y, new WorldTileData(_debugTile), true);

            //execute = false;
        }
    }
}
