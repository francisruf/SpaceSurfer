using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugWindController : MonoBehaviour
{
    // Internal logic
    private int _windDirection;

    private void OnEnable()
    {
        DebugManager.OnWindDirectionRequest += SetWindDirection;
    }

    private void OnDisable()
    {
        DebugManager.OnWindDirectionRequest -= SetWindDirection;
    }

    private void SetWindDirection(int directionAngle)
    {
        _windDirection = -directionAngle % 360;
        transform.rotation = Quaternion.Euler(0f, 0f, _windDirection);
    }
}
