using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GridCoordTester : MonoBehaviour
{

}

[CustomEditor(typeof(GridCoordTester))]
public class GridCoordEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GridCoordTester tester = (GridCoordTester)target;

        Grid grid = tester.GetComponentInParent<Grid>();

        if (grid != null)
        {
            Vector2Int gridCoord = (Vector2Int)grid.WorldToCell(tester.transform.position);
            EditorGUILayout.LabelField("Gird Coords : ", gridCoord.ToString());
        }

    }
}
