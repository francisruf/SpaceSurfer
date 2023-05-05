using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine.Utility;


public class Wind_Linear : Wind_Base
{
    [SerializeField] private Transform _start;
    [SerializeField] private Transform _end;
    [SerializeField] private float _cellSize;
    [SerializeField] private float _width = 1f;
    [SerializeField] private float _windStrength;

    [SerializeField] private BoxCollider2D _collider;

    public override List<WindForce> GenerateWindUpdate()
    {
        return _windForces;
    }

    public override void Initialize(WindGrid grid)
    {
        base.Initialize(grid);

        List<Vector3> worldPositions = GetWorldPositions();
        SetWindForces(worldPositions);
    }

    private List<Vector3> GetWorldPositions()
    {
        List<Vector3> allPositions = new List<Vector3>();

        Vector3 diff = _end.transform.position - _start.transform.position;
        Vector2 dir = diff.normalized;
        Vector3 perp = Vector2.Perpendicular(diff).normalized;

        _collider.size = new Vector2(diff.magnitude, _width);
        _collider.transform.position = _start.transform.position + (diff / 2f);

        float angle = Vector2.SignedAngle(diff, Vector2.right);
        _collider.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, -angle));

        Vector3 corner0 = _start.transform.position - perp * (_width / 2f);
        Vector3 corner1 = _start.transform.position + perp * (_width / 2f);
        Vector3 corner2 = _end.transform.position + perp * (_width / 2f);
        Vector3 corner3 = _end.transform.position - perp * (_width / 2f);
        Vector3[] corners = new Vector3[4] { corner0, corner1, corner2, corner3 };

        float xMin = float.MaxValue;
        float xMax = float.MinValue;
        float yMin = float.MaxValue;
        float yMax = float.MinValue;

        for (int i = 0; i < corners.Length; i++)
        {
            if (corners[i].x < xMin)
                xMin = corners[i].x;
            if (corners[i].x > xMax)
                xMax = corners[i].x;
            if (corners[i].y < yMin)
                yMin = corners[i].y;
            if (corners[i].y > yMax)
                yMax = corners[i].y;
        }

        xMin += _cellSize - xMin % _cellSize;
        xMax += _cellSize - xMax % _cellSize;
        yMin += _cellSize - yMin % _cellSize;
        yMax += _cellSize - yMax % _cellSize;

        for (float x = xMin; x < xMax; x += _cellSize)
        {
            for (float y = yMin; y < yMax; y += _cellSize)
            {
                if (_collider.OverlapPoint(new Vector2(x, y)))
                {
                    allPositions.Add(new Vector3(x, y));
                }
            }
        }
        return allPositions;
    }

    private void SetWindForces(List<Vector3> worldPositions)
    {
        _windForces.Clear();
        Vector2 dir = (_end.transform.position - _start.transform.position).normalized;

        foreach (var pos in worldPositions)
        {
            Vector2Int CellPos = (Vector2Int)_windGrid.WorldToCell(pos);
            _windForces.Add(new WindForce(CellPos, _windStrength, dir));
        }
    }

    //private void CalculateGridPositionsLegacy()
    //{
    //    _windForces.Clear();

    //    Vector3 diff = _end.transform.position - _start.transform.position;
    //    Vector3 perp = Vector2.Perpendicular(diff).normalized;

    //    int lenght = Mathf.CeilToInt(diff.magnitude / _cellSize);
    //    int width = Mathf.CeilToInt(_width / _cellSize);
    //    Vector3 pos = new Vector3();
    //    Vector3 dir = new Vector3();

    //    for (int l = 0; l < lenght; l++)
    //    {
    //        for (int w = 0; w <= width; w++)
    //        {
    //            Vector3 origin = _start.transform.position - perp * (_width / 2f);
    //            pos = origin + (diff.normalized * _cellSize * l) + (perp * _cellSize * w);

    //            pos.x -= pos.x % _cellSize;
    //            pos.y -= pos.y % _cellSize;

    //            dir = (_end.transform.position - _start.transform.position).normalized;

    //            Vector2Int CellPos = (Vector2Int)_windGrid.WorldToCell(pos);
    //            _windForces.Add(new WindForce(CellPos, _windStrength, dir));
    //        }
    //    }
    //}


    private void OnDrawGizmos()
    {
        DrawDebug();
    }

    private void DrawDebug()
    {
        Vector3 diff = _end.transform.position - _start.transform.position;
        Vector3 perp = Vector2.Perpendicular(diff).normalized;
        Vector3 corner0 = _start.transform.position - perp * (_width / 2f);
        Vector3 corner1 = _start.transform.position + perp * (_width / 2f);
        Vector3 corner2 = _end.transform.position + perp * (_width / 2f);
        Vector3 corner3 = _end.transform.position - perp * (_width / 2f);
        Vector3[] corners = new Vector3[4] { corner0, corner1, corner2, corner3 };

        Gizmos.DrawLine(corners[0], corners[1]);
        Gizmos.DrawLine(corners[1], corners[2]);
        Gizmos.DrawLine(corners[2], corners[3]);
        Gizmos.DrawLine(corners[3], corners[0]);

        foreach (var pos in GetWorldPositions())
        {
            Gizmos.DrawSphere(pos, 0.10f);
        }
    }

    //private void DrawBetterSpheres()
    //{
    //    Vector3 diff = _end.transform.position - _start.transform.position;
    //    Vector3 perp = Vector2.Perpendicular(diff).normalized;

    //    _collider.size = new Vector2(diff.magnitude, _width);
    //    _collider.transform.position = _start.transform.position + (diff / 2f);

    //    float angle = Vector2.SignedAngle(diff, Vector2.right);
    //    _collider.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, -angle));

    //    Vector3 corner0 = _start.transform.position - perp * (_width / 2f);
    //    Vector3 corner1 = _start.transform.position + perp * (_width / 2f);
    //    Vector3 corner2 = _end.transform.position + perp * (_width / 2f);
    //    Vector3 corner3 = _end.transform.position - perp * (_width / 2f);
    //    Vector3[] corners = new Vector3[4] { corner0, corner1, corner2, corner3 };

    //    float xMin = float.MaxValue;
    //    float xMax = float.MinValue;
    //    float yMin = float.MaxValue;
    //    float yMax = float.MinValue;

    //    for (int i = 0; i < corners.Length; i++)
    //    {
    //        if (corners[i].x < xMin)
    //            xMin = corners[i].x;
    //        if (corners[i].x > xMax)
    //            xMax = corners[i].x;
    //        if (corners[i].y < yMin)
    //            yMin = corners[i].y;
    //        if (corners[i].y > yMax)
    //            yMax = corners[i].y;
    //    }

    //    xMin += _cellSize - xMin % _cellSize;
    //    xMax += _cellSize - xMax % _cellSize;
    //    yMin += _cellSize - yMin % _cellSize;
    //    yMax += _cellSize - yMax % _cellSize;

    //    for (float x = xMin; x < xMax; x += _cellSize)
    //    {
    //        for (float y = yMin; y < yMax; y += _cellSize)
    //        {
    //            if (_collider.OverlapPoint(new Vector2(x,y)))
    //                Gizmos.DrawSphere(new Vector3(x, y), 0.1f);
    //        }
    //    }

    //    Gizmos.DrawLine(corners[0], corners[1]);
    //    Gizmos.DrawLine(corners[1], corners[2]);
    //    Gizmos.DrawLine(corners[2], corners[3]);
    //    Gizmos.DrawLine(corners[3], corners[0]);

    //}
}
