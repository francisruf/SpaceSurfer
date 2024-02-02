using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ChunkData<T>
{
    public int chunkX;
    public int chunkY;
    public T[,] tileData;

    private int lenghtX;
    private int lenghtY;

    //public TileBase[,] tilmapTiles;

    public ChunkData(int chunkX, int chunkY)
    {
        this.chunkX = chunkX;
        this.chunkY = chunkY;

        lenghtX = tileData.GetLength(0);
        lenghtY = tileData.GetLength(1);
    }

    public ChunkData(int chunkX, int chunkY, T[,] tileData)
    {
        this.chunkX = chunkX;
        this.chunkY = chunkY;
        this.tileData = tileData;

        lenghtX = tileData.GetLength(0);
        lenghtY = tileData.GetLength(1);
    }

    public void SetTileData(int x, int y, T tileData)
    {
        if (x > lenghtX - 1 || y > lenghtY - 1 || x < 0 || y < 0)
        {
            Debug.LogError("Attempted to set tile data outside of chunk bounds.");
            return;
        }
        this.tileData[x, y] = tileData;
    }

    public T GetTileData(int x, int y)
    {
        if (x > lenghtX - 1 || y > lenghtY - 1 || x < 0 || y < 0)
        {
            return default(T);
        }
        return tileData[x,y];
    }
}
