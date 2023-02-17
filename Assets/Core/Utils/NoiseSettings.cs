using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Noise Settings", menuName = "ScriptableObjects/Noise Settings")]
public class NoiseSettings : ScriptableObject
{
    [Header("Debug Settings")]
    // TODO : Replace with a call from the WindMap class
    public int mapWidth = 100;
    public int mapHeight = 100;
    public int octaves = 1;
    public Vector2 noiseScale = new Vector2(1f, 1f);

    [Tooltip("Controls decrease in amplitude of octaves. Further octaves have less influence on the final result." +
        "\nOn a map : Increasing persistance increases the influence of these small features")]
    [Range(0.0f, 1.0f)]
    public float persistance = 0.5f;

    [Tooltip("Controls increase in frequency of octaves.\nOn a map : Increases the number of small features.")]
    public float lacunarity = 1;
}
