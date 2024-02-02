using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New World Settings", menuName = "ScriptableObjects/World Settings")]
public class WorldSettings : ScriptableObject
{
    public int gridCellSize = 1;
    public int chunkSize = 5;
    public int chunkRenderDistance = 2;
    public WorldType worldType = WorldType.Handmade;
}

[System.Serializable]
public enum WorldType
{
    Handmade,
    ProceduralEmpty,
    ProceduralObstacles
}