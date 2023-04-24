using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherGrid : GridBase<WeatherTileData>
{
    protected override WeatherTileData[,] CreateTileData(int chunkX, int chunkY)
    {
        return new WeatherTileData[_chunkSize, _chunkSize];
    }

    protected override void LoadChunk(ChunkData<WeatherTileData> chunkData)
    {
        // throw new System.NotImplementedException();
    }

    protected override void UnloadChunk(ChunkData<WeatherTileData> chunkData)
    {
        // throw new System.NotImplementedException();
    }

    protected override void UpdateChunks()
    {
        throw new System.NotImplementedException();
    }
}
