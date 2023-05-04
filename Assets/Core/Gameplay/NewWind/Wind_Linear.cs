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
        CalculateGridPositions();
    }

    private void CalculateGridPositions()
    {
        _windForces.Clear();

        Vector3 diff = _end.transform.position - _start.transform.position;
        Vector3 perp = Vector2.Perpendicular(diff).normalized;

        int lenght = Mathf.CeilToInt(diff.magnitude / _cellSize);
        int width = Mathf.CeilToInt(_width / _cellSize);
        Vector3 pos = new Vector3();
        Vector3 dir = new Vector3();

        for (int l = 0; l < lenght; l++)
        {
            for (int w = 0; w <= width; w++)
            {
                Vector3 origin = _start.transform.position - perp * (_width / 2f);
                pos = origin + (diff.normalized * _cellSize * l) + (perp * _cellSize * w);

                pos.x -= pos.x % _cellSize;
                pos.y -= pos.y % _cellSize;

                dir = (_end.transform.position - _start.transform.position).normalized;

                Vector2Int CellPos = (Vector2Int)_windGrid.WorldToCell(pos);
                _windForces.Add(new WindForce(CellPos, _windStrength, dir));
            }
        }
    }


    private void OnDrawGizmos()
    {
        //DrawApproximateSpheres();
        DrawBetterSpheres();
    }

    private void DrawBetterSpheres()
    {
        Vector3 diff = _end.transform.position - _start.transform.position;
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
                if (_collider.OverlapPoint(new Vector2(x,y)))
                    Gizmos.DrawSphere(new Vector3(x, y), 0.1f);
            }
        }

        Gizmos.DrawLine(corners[0], corners[1]);
        Gizmos.DrawLine(corners[1], corners[2]);
        Gizmos.DrawLine(corners[2], corners[3]);
        Gizmos.DrawLine(corners[3], corners[0]);

    }

    // Previous version
    private void DrawApproximateSpheres()
    {
        Vector3 diff = _end.transform.position - _start.transform.position;
        Vector3 perp = Vector2.Perpendicular(diff).normalized;

        Vector3 corner0 = _start.transform.position - perp * (_width / 2f);
        Vector3 corner1 = _start.transform.position + perp * (_width / 2f);
        Vector3 corner2 = _end.transform.position + perp * (_width / 2f);
        Vector3 corner3 = _end.transform.position - perp * (_width / 2f);
        Vector3[] corners = new Vector3[4] { corner0, corner1, corner2, corner3 };

        int lenght = Mathf.CeilToInt(diff.magnitude / _cellSize);
        int width = Mathf.CeilToInt(_width / _cellSize);
        Vector3 pos = new Vector3();

        for (int l = 0; l < lenght; l++)
        {
            for (int w = 0; w <= width; w++)
            {
                Vector3 origin = corner0;
                pos = origin + (diff.normalized * _cellSize * l) + (perp * _cellSize * w);

                pos.x -= pos.x % _cellSize;
                pos.y -= pos.y % _cellSize;

                Gizmos.DrawSphere(pos, 0.1f);
            }
        }

        Gizmos.DrawLine(corners[0], corners[1]);
        Gizmos.DrawLine(corners[1], corners[2]);
        Gizmos.DrawLine(corners[2], corners[3]);
        Gizmos.DrawLine(corners[3], corners[0]);
        //Gizmos.DrawLine(_start.position, _end.position);
    }

    private void DrawSpheresWIP()
    {
        Vector3 diff = _end.transform.position - _start.transform.position;
        Vector3 perp = Vector2.Perpendicular(diff).normalized;

        Vector3 corner0 = _start.transform.position - perp * (_width / 2f);
        Vector3 corner1 = _start.transform.position + perp * (_width / 2f);
        Vector3 corner2 = _end.transform.position + perp * (_width / 2f);
        Vector3 corner3 = _end.transform.position - perp * (_width / 2f);

        Vector3[] corners = new Vector3[4] { corner0, corner1, corner2, corner3 };
        float minX = corners[0].x;
        int minIndex = 0;

        for (int i = 1; i < corners.Length; i++)
        {
            if (corners[i].x < minX)
            {
                minX = corners[i].x;
                minIndex = i;
            }
        }

        Vector3[] sortedCorners = new Vector3[4];
        for (int i = 0; i < sortedCorners.Length; i++)
        {
            sortedCorners[i] = corners[(i + minIndex) % corners.Length];
        }

        Gizmos.DrawLine(sortedCorners[0], sortedCorners[1]);
        Gizmos.DrawLine(sortedCorners[1], sortedCorners[2]);
        Gizmos.DrawLine(sortedCorners[2], sortedCorners[3]);
        Gizmos.DrawLine(sortedCorners[3], sortedCorners[0]);

        Vector2 origin = sortedCorners[0];
        origin.x -= origin.x % _cellSize;
        origin.y -= origin.y % _cellSize;
        Vector2 current = origin;

        Gizmos.DrawCube(origin, Vector3.one * 0.2f);

        Vector2 intersectRight0;
        Vector2 intersectRight1 = sortedCorners[2];
        Vector2 intersectRightMax = new Vector2(sortedCorners[2].x, origin.y);

        if (sortedCorners[2].y < origin.y)
            intersectRight0 = sortedCorners[1];
        else
            intersectRight0 = sortedCorners[3];

        Vector2 intersectPointRight;
        UnityVectorExtensions.FindIntersection(origin, intersectRightMax, intersectRight0, intersectRight1, out intersectPointRight);
        Gizmos.DrawCube(intersectPointRight, Vector3.one * 0.25f);

        Vector2 intersectUp0 = sortedCorners[0];
        Vector2 intersectUp1 = sortedCorners[1];
        Vector2 intersectUpMax = new Vector2(origin.x, sortedCorners[1].y);
        Vector2 intersectPointUp;
        UnityVectorExtensions.FindIntersection(origin, intersectUpMax, intersectUp0, intersectUp1, out intersectPointUp);

        //while (current.x < intersectPointRight.x)
        //{
        //    Gizmos.DrawSphere(current, 0.15f);
        //    while (current.y < intersectPointUp.y)
        //    {
        //        Gizmos.DrawSphere(current, 0.15f);


        //        intersectUpMax = new Vector2(current.x, sortedCorners[1].y);
        //        UnityVectorExtensions.FindIntersection(new Vector2(current.x, origin.y), intersectUpMax, intersectUp0, intersectUp1, out intersectPointUp);
        //        current.y += _cellSize;
        //    }
        //    current.y = origin.y;
        //    current.x += _cellSize;
        //}



        //Vector3 highest = corner0;
        //Vector3 lowest = corner0;

        //for (int i = 1; i < 4; i++)
        //{
        //    if (corners[i].y > highest.y)
        //        highest = corners[i];
        //}

        //for (int i = 1; i < 4; i++)
        //{
        //    if (corners[i].y < lowest.y)
        //        lowest = corners[i];
        //}

        //Gizmos.DrawLine(highest, lowest);


        //float yMin = highest.y - _width;
        //float columns = highest.y - (highest.y % _cellSize);

        //while (columns >= yMin)
        //{
        //    Gizmos.DrawSphere(new Vector2(highest.x, columns), 0.1f);
        //    columns -= _cellSize;
        //}

        //Vector2 intersect;
        //UnityVectorExtensions.FindIntersection(highest, new Vector2(highest.x, lowest.y), lowest, corner3, out intersect);
        //Gizmos.DrawCube(intersect, Vector3.one * 0.25f);


        //Gizmos.DrawLine(_end.transform.position - perp * (_width / 2f), _end.transform.position + perp * (_width / 2f));
        //Gizmos.DrawLine(_start.transform.position - perp * (_width / 2f), _start.transform.position + perp * (_width / 2f));
        //Gizmos.DrawLine(_start.transform.position + perp * (_width / 2f), _end.transform.position + perp * (_width / 2f));
        //Gizmos.DrawLine(_start.transform.position - perp * (_width / 2f), _end.transform.position - perp * (_width / 2f));
    }

}
