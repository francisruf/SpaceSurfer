using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GlobalObstacleController))]
public class GlobalObstacleControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        GlobalObstacleController controller = (GlobalObstacleController)target;
        if (DrawDefaultInspector())
        {
        }

        if (GUILayout.Button("Generate test"))
            controller.GenerateDebugMap();

        if (GUILayout.Button("Cleanup Map"))
            controller.CleanUpDebugMap();
    }
}