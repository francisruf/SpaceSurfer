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
        Vector3 diff = _end.transform.position - _start.transform.position;
        Vector3 perp = Vector2.Perpendicular(diff).normalized;

        Vector3 corner0 = _start.transform.position - perp * (_width / 2f);
        Vector3 corner1 = _start.transform.position + perp * (_width / 2f);
        Vector3 corner2 = _end.transform.position + perp * (_width / 2f);
        Vector3 corner3 = _end.transform.position - perp * (_width / 2f);
        Vector3[] corners = new Vector3[4] { corner0, corner1, corner2, corner3 };

        Gizmos.DrawLine(corner0, corner1);
        Gizmos.DrawLine(corner1, corner2);
        Gizmos.DrawLine(corner2, corner3);
        Gizmos.DrawLine(corner3, corner0);

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


        //Gizmos.DrawLine(_end.transform.position - perp * (_width/2f), _end.transform.position + perp * (_width / 2f));
        //Gizmos.DrawLine(_start.transform.position - perp * (_width / 2f), _start.transform.position + perp * (_width / 2f));
        //Gizmos.DrawLine(_start.transform.position + perp * (_width / 2f), _end.transform.position + perp * (_width / 2f));
        //Gizmos.DrawLine(_start.transform.position - perp * (_width / 2f), _end.transform.position - perp * (_width / 2f));

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
    }
}
