using LandmassGeneration;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LandmassGeneration
{
    public class MapGenerator : MonoBehaviour
    {
        public EDrawMode drawMode;

        public const int mapChunkSize = 241;
        [Range(0,6)]
        public int levelOfDetail;
        public Vector2 noiseScale;

        public int octaves;
        [Tooltip("Controls decrease in amplitude of octaves. Further octaves have less influence on the final result.\nOn a map : Increasing persistance increases the influence of these small features")]
        [Range(0.0f, 1.0f)]
        public float persistance;
        [Tooltip("Controls increase in frequency of octaves.\nOn a map : Increases the number of small features.")]
        public float lacunarity;

        public int seed;
        public Vector2 offset;

        public float meshHeightMultiplier = 1f;
        public AnimationCurve meshHeightCurve;

        public bool autoUpdate;

        public STerrainType[] regions;

        public void GenerateMap()
        {
            float[,] noiseMap = Noise.GenerateNoiseMap(mapChunkSize, mapChunkSize, seed, noiseScale, octaves, persistance, lacunarity, offset);
            Color[] colourMap = new Color[mapChunkSize * mapChunkSize];

            for (int y = 0; y < mapChunkSize; y++)
            {
                for (int x = 0; x < mapChunkSize; x++)
                {
                    float currentHeight = noiseMap[x, y];
                    for (int i = 0; i < regions.Length; i++)
                    {
                        if (currentHeight <= regions[i].height)
                        {
                            colourMap[y * mapChunkSize + x] = regions[i].colour;
                            break;
                        }
                    }
                }
            }
            MapDisplay mapDisplay = FindObjectOfType<MapDisplay>();

            switch (drawMode)
            {
                case EDrawMode.NoiseMap:
                    mapDisplay.DrawTexture(TextureGenerator.TextureFromHeightMap(noiseMap));
                    break;
                case EDrawMode.ColourMap:
                    mapDisplay.DrawTexture(TextureGenerator.TextureFromColourMap(colourMap, mapChunkSize, mapChunkSize));
                    break;
                case EDrawMode.Mesh:
                    mapDisplay.DrawMesh(MeshGenerator.GenerateTerrainMesh(noiseMap, meshHeightMultiplier, meshHeightCurve, levelOfDetail), TextureGenerator.TextureFromColourMap(colourMap, mapChunkSize, mapChunkSize));
                    break;
            }
        }

        private void OnValidate()
        {
            if (lacunarity < 1)
                lacunarity = 1;
            if (octaves < 0)
                octaves = 0;
        }
    }
}

