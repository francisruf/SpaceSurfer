using LandmassGeneration;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LandmassGeneration
{
    public class EndlessTerrain : MonoBehaviour
    {
        public const float maxViewDist = 450f;
        public Transform viewer;

        public static Vector2 viewerPosition;
        private int chunkSize;
        private int chunksVisibleInViewDist;

        private Dictionary<Vector2, TerrainChunk> terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();
        private List<TerrainChunk> terrainChunksVisibleLastUpdate = new List<TerrainChunk>();

        private void Start()
        {
            chunkSize = MapGenerator.mapChunkSize - 1;
            chunksVisibleInViewDist = Mathf.RoundToInt(maxViewDist / chunkSize);
        }

        private void Update()
        {
            viewerPosition = new Vector2(viewer.position.x, viewer.position.z);
            UpdateVisibleChunks();
        }

        private void UpdateVisibleChunks()
        {
            for (int i = 0; i < terrainChunksVisibleLastUpdate.Count; i++)
            {
                terrainChunksVisibleLastUpdate[i].SetVisible(false);
            }
            terrainChunksVisibleLastUpdate.Clear();

            int currentChunkCoordX = Mathf.RoundToInt(viewerPosition.x / chunkSize);
            int currentChunkCoordY = Mathf.RoundToInt(viewerPosition.y / chunkSize);

            for (int yOffset = -chunksVisibleInViewDist; yOffset <= chunksVisibleInViewDist; yOffset++)
                for (int xOffset = -chunksVisibleInViewDist; xOffset <= chunksVisibleInViewDist; xOffset++)
                {
                    Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);

                    if (terrainChunkDictionary.ContainsKey(viewedChunkCoord))
                    {
                        terrainChunkDictionary[viewedChunkCoord].UpdateTerrainChunk();
                    }
                    else
                    {
                        terrainChunkDictionary.Add(viewedChunkCoord, new TerrainChunk(viewedChunkCoord, chunkSize, this.transform));
                    }

                    terrainChunksVisibleLastUpdate.Add(terrainChunkDictionary[viewedChunkCoord]);
                }
        }
        public class TerrainChunk
        {
            GameObject meshObject;
            Vector2 position;
            Bounds bounds;

            public TerrainChunk(Vector2 coord, int size, Transform parent)
            {
                this.position = coord * size;
                bounds = new Bounds(position, Vector2.one * size);
                Vector3 positionV3 = new Vector3(position.x, 0, position.y);

                meshObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
                meshObject.transform.position = positionV3;
                meshObject.transform.localScale = Vector3.one * size / 10f;    // Plane's default scale is 10
                meshObject.transform.parent = parent;
                SetVisible(false);
            }
            public void UpdateTerrainChunk()
            {
                float viewerDistFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));
                bool visible = viewerDistFromNearestEdge <= maxViewDist;
                SetVisible(visible);
            }

            public void SetVisible(bool visible)
            {
                meshObject.SetActive(visible);
            }

            public bool isVisible()
            {
                return meshObject.activeSelf;
            }
        }
    }



}