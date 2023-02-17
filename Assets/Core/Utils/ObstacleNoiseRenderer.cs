using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(GlobalObstacleController))]
public class ObstacleNoiseRenderer : NoiseRenderer
{
    [SerializeField] private GlobalObstacleController _obstacleController;

    protected override TileBase SelectTileByNoise(float noiseValue)
    {
        int length = _obstacleController.Tiles.Length;
        
        if (length == 0)
            return null;

        for (int i = 0; i < length; i++)
        {
            if (_obstacleController.Tiles[i].value >= noiseValue)
                return _obstacleController.Tiles[i].tile;
        }
        return null;
    }
}

