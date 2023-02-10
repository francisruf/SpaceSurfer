using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshGenerator
{
    public static MeshData GenerateTerrainMesh(float[,] heightMap, float heightMultiplier, AnimationCurve heightCurve, int levelOfDetail)
    {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);

        // Centers the mesh.
        // remove 1 value (for the median 0.0 value), and divide by minus half, to get the leftmost point
        float topLeftX = (width - 1) / -2f;
        // remove 1 value (for the median 0.0 value), and divid by positive half, to get topmost point
        float topLeftZ = (height - 1) / 2f;

        int meshSimplificationIncrement = (levelOfDetail == 0) ? 1 : levelOfDetail * 2;
        int verticesPerLine = ((width - 1) / meshSimplificationIncrement) + 1;

        MeshData meshData = new MeshData(verticesPerLine, verticesPerLine);
        int vertexIndex = 0;

        // MeshSimplificationIncrement = LOD factor. If not using LODs, simply increment by 1.
        for (int y = 0; y < height; y += meshSimplificationIncrement)
        {
            for (int x = 0; x < width; x += meshSimplificationIncrement)
            {
                meshData.vertices[vertexIndex] = new Vector3(topLeftX + x, heightCurve.Evaluate(heightMap[x,y]) * heightMultiplier, topLeftZ - y);
                meshData.uvs[vertexIndex] = new Vector2(x / (float)width, y / (float)height);

                // Ignoring right and bottom edges of the grid,
                // since previous indexes will already have used their vertices to form triangles
                if (x < width - 1 && y < height - 1)
                {
                    meshData.AddTriangle(vertexIndex, vertexIndex + verticesPerLine + 1, vertexIndex + verticesPerLine);
                    meshData.AddTriangle(vertexIndex + verticesPerLine + 1, vertexIndex, vertexIndex + 1);
                }

                vertexIndex++;
            }
        }
        return meshData;
    }
}

public class MeshData
{
    public Vector3[] vertices;
    public int[] triangles;
    public Vector2[] uvs;

    int triangleIndex;

    public MeshData(int meshWidth, int meshHeight)
    {
        this.vertices = new Vector3[meshWidth * meshHeight];
        this.uvs = new Vector2[meshWidth * meshHeight];
        this.triangles = new int[((meshWidth - 1) * (meshHeight - 1)) * 6];
    }

    public void AddTriangle(int a, int b, int c)
    {
        triangles[triangleIndex] = a;
        triangles[triangleIndex + 1] = b;
        triangles[triangleIndex + 2] = c;
        triangleIndex += 3;
    }

    public Mesh CreateMesh()
    {
        Mesh mesh = new Mesh();

        // World Position of the vertex
        mesh.vertices = vertices;
        // 1D array, of all the triangles formed by the vertices
        // (one value = one vertex, so a single triangle has 3 entries in the array)
        mesh.triangles = triangles;
        // UV coordinates of each vertex, in texture percentage
        mesh.uv = uvs;
        mesh.RecalculateNormals();
        return mesh;
    }
}
