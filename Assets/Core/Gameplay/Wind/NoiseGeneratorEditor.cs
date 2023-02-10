using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(NoiseGenerator))]
public class NoiseGeneratorEditor : Editor
{
    //public override void OnInspectorGUI()
    //{
    //    NoiseGenerator noiseGenerator = (NoiseGenerator)target;

    //    if (DrawDefaultInspector())
    //    {
    //        noiseGenerator.GenerateDebugMap();
    //    }

    //    if (GUILayout.Button("Generate"))
    //        noiseGenerator.GenerateDebugMap();
    //}
}
