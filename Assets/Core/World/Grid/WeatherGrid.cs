using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherGrid : GridBase<WeatherTileData>
{
    public WeatherSystem[] debugStartingWeather;

    protected override void Start()
    {
        base.Start();
        InitializeWeatherSystems();
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void OnEnable()
    {
        Sail.onNewSail += HandleNewSail;
    }

    protected override void OnDisable()
    {
        Sail.onNewSail -= HandleNewSail;
    }

    private void HandleNewSail(IGridActor<WeatherTileData> sail)
    {
        RegisterGridActor(sail);
    }

    private void InitializeWeatherSystems()
    {
        for (int i = 0; i < debugStartingWeather.Length; i++)
        {
            debugStartingWeather[i].Initialize(this);
        }
    }

    public void SetWeatherSystem(Vector2Int[] gridCoords, WeatherSystem weather)
    {
        foreach (var coord in gridCoords)
        {
            WeatherTileData data = GridToTile(coord.x, coord.y);
            if (data == null)
            {
                data = new WeatherTileData();
                SetTileData(coord.x, coord.y, data);
            }
            data.AddWeatherSystem(weather);
        }
    }

    protected override WeatherTileData[,] CreateTileData(int chunkX, int chunkY)
    {
        return new WeatherTileData[_chunkSizeX, _chunkSizeY];
    }

    protected override void LoadChunk(ChunkData<WeatherTileData> chunkData)
    {
    }

    protected override void UnloadChunk(ChunkData<WeatherTileData> chunkData)
    {
    }

    protected override void UpdateChunks()
    {
    }
}
