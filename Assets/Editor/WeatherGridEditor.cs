using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(LegacyWeatherGrid))]
public class WeatherGridEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        LegacyWeatherGrid weatherGrid = (LegacyWeatherGrid)target;
        weatherGrid.TilemapGrid.cellSize = new Vector3(weatherGrid.gridCellSize, weatherGrid.gridCellSize);
    }
}
