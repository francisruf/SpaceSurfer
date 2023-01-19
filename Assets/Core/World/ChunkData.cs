using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ChunkData
{
    public int chunkX;
    public int chunkY;
    public TileBase[,] tiles;

    public ChunkData(int chunkX, int chunkY, TileBase[,] tiles)
    {
        this.chunkX = chunkX;
        this.chunkY = chunkY;
        this.tiles = tiles;
    }
}
