using LandmassGeneration;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LandmassGeneration
{
    public static class Noise
    {
        public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, int seed, Vector2 noiseScale, int octaves, float persistance, float lacunarity, Vector2 offset)
        {
            // Pseudo-Random Number Generator
            System.Random prng = new System.Random(seed);

            Vector2[] octaveOffsets = new Vector2[octaves];
            for (int i = 0; i < octaves; i++)
            {
                float offsetX = prng.Next(-100000, 100000) + offset.x;
                float offsetY = prng.Next(-100000, 100000) + offset.y;
                octaveOffsets[i] = new Vector2(offsetX, offsetY);
            }

            float[,] noiseMap = new float[mapWidth, mapHeight];

            // Prevent division by 0
            if (noiseScale.x <= 0)
                noiseScale.x = 0.0001f;

            if (noiseScale.y <= 0)
                noiseScale.y = 0.0001f;

            float maxNoiseHeight = float.MinValue;
            float minNoiseHeight = float.MaxValue;

            float halfWidth = mapWidth / 2f;
            float halfHeight = mapHeight / 2f;

            // Find perlin values and add to noise map[,]
            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    float amplitude = 1;
                    float frequency = 1;
                    float noiseHeight = 0;

                    for (int i = 0; i < octaves; i++)
                    {
                        // Avoids using Integral coords (1-2-3), as the PerlinNoise unity func will always return same value (an average in the middle of the coords)
                        float sampleX = (x - halfWidth) / noiseScale.x * frequency + octaveOffsets[i].x;
                        float sampleY = (y - halfHeight) / noiseScale.y * frequency + octaveOffsets[i].y;

                        // * 2 - 1  --> Perlin only returns 0 to 1. * 2 - 1 allows to get values from -1 to +1
                        float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                        noiseHeight += perlinValue * amplitude;

                        // amplitude decreases each octave, since persistance should be less than 1
                        amplitude *= persistance;
                        // Frequency increases each octave, since lacunarity should be greater than 1
                        frequency *= lacunarity;
                    }
                    if (noiseHeight > maxNoiseHeight)
                        maxNoiseHeight = noiseHeight;
                    else if (noiseHeight < minNoiseHeight)
                        minNoiseHeight = noiseHeight;

                    noiseMap[x, y] = noiseHeight;
                }
            }

            // Normalize noise map back to 0-1
            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    // Inverse Lerp : Takes a value, and lerps between a min-max to return an alpha value (0-1)
                    noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
                }
            }

            return noiseMap;
        }
    }
}

