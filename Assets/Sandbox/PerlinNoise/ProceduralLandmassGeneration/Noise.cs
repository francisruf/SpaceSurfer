using LandmassGeneration;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LandmassGeneration
{
    public static class Noise
    {
        public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, float scale)
        {
            float[,] noiseMap = new float[mapWidth, mapHeight];

            // Prevent division by 0
            if (scale <= 0)
                scale = 0.0001f;

            // Find perlin values and add to noise map[,]
            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    // Avoids using Integral coords (1-2-3), as the PerlinNoise unity func will always return same value (an average in the middle of the coords)
                    float sampleX = x / scale;
                    float sampleY = y / scale;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY);
                    noiseMap[x, y] = perlinValue;
                }
            }
            return noiseMap;
        }
    }
}

