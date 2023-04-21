using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

public static class NoiseGenerator
{
    // TODO : Add overrides for static maps

    public static float[,] GenerateNoiseMap(NoiseSettings settings, Vector2[] octaveOffsets, Vector3Int globalOffset, float time, Vector2 timeOffsetDirection, bool normalizeNoise)
    {
        float[,] noiseMap = new float[settings.mapWidth, settings.mapHeight];
        // Prevent division by 0
        if (settings.noiseScale.x <= 0)
            settings.noiseScale.x = 0.0001f;

        if (settings.noiseScale.y <= 0)
            settings.noiseScale.y = 0.0001f;

        float halfWidth = settings.mapWidth / 2f;
        float halfHeight = settings.mapHeight / 2f;

        float minNoiseHeight = float.MaxValue;
        float maxNoiseHeight = float.MinValue;

        for (int x = 0; x < settings.mapWidth; x++)
        {
            for (int y = 0; y < settings.mapHeight; y++)
            {
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                for (int i = 0; i < settings.octaves; i++)
                {
                    float sampleX = (x - halfWidth + globalOffset.x) / settings.noiseScale.x * frequency + octaveOffsets[i].x + -time * timeOffsetDirection.normalized.x;
                    float sampleY = (y - halfHeight + globalOffset.y) / settings.noiseScale.y * frequency + octaveOffsets[i].y + -time * timeOffsetDirection.normalized.y;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY);
                    noiseHeight += perlinValue * amplitude;

                    if (noiseHeight < minNoiseHeight)
                        minNoiseHeight = perlinValue;

                    else if (noiseHeight > maxNoiseHeight)
                        maxNoiseHeight = perlinValue;

                    // amplitude decreases each octave, since persistance should be less than 1
                    amplitude *= settings.persistance;
                    // Frequency increases each octave, since lacunarity should be greater than 1
                    frequency *= settings.lacunarity;

                    noiseMap[x, y] = noiseHeight;
                }
            }
        }

        if (normalizeNoise)
        {
            for (int x = 0; x < settings.mapWidth; x++)
            {
                for (int y = 0; y < settings.mapHeight; y++)
                {
                    noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
                }
            }
        }

        return noiseMap;
    }

    public static Vector2[] GenerateOctaveOffsets(int octaveCount)
    {
        System.Random prng = new System.Random();
        Vector2[] octaveOffsets = new Vector2[octaveCount];

        for (int i = 0; i < octaveCount; i++)
        {
            float offsetX = prng.Next(-100000, 100000);
            float offsetY = prng.Next(-100000, 100000);
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }

        return octaveOffsets;
    }
}


