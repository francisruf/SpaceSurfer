using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    public static Action<Signature> onSignatureDetected;
    public static Action onSignatureEmpty;

    [SerializeField] private LayerMask _layerMask;

    private Mesh _mesh;
    private CustomCollider2D _collider;

    [SerializeField] private float _fov = 90f;
    [SerializeField] private int _rayCount = 20;
    [SerializeField] private float _viewDistance = 2f;
    private float _angleIncrease;
    private float _startingAngle;
    private Vector3 _origin = Vector3.zero;

    private ResonanceObject _currentResonanceObject;

    private void Awake()
    {
        _collider = GetComponent<CustomCollider2D>();
        _mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = _mesh;
    }

    public void UpdateFieldOfView()
    {
        float angle = _startingAngle;
        _angleIncrease = _fov / _rayCount;

        Vector3[] vertices = new Vector3[_rayCount + 1 + 1];
        Vector2[] uv = new Vector2[vertices.Length];
        int[] triangles = new int[_rayCount * 3];

        vertices[0] = _origin;

        int vertexIndex = 1;
        int triangleIndex = 0;
        int polygonIndex = 0;

        PhysicsShapeGroup2D physicsShapeGroup2D = new PhysicsShapeGroup2D(triangles.Length, vertices.Length);
        
        for (int i = 0; i <= _rayCount; i++)
        {
            Vector3 vertex = _origin + GetVectorFromAngle(angle) * _viewDistance;
            vertices[vertexIndex] = vertex;

            if (i > 0)
            {
                triangles[triangleIndex + 0] = 0;
                triangles[triangleIndex + 1] = vertexIndex - 1;
                triangles[triangleIndex + 2] = vertexIndex;

                triangleIndex += 3;

                List<Vector2> polygonVertices = new List<Vector2>();
                polygonVertices.Add(_origin);
                polygonVertices.Add(vertices[vertexIndex - 1]);
                polygonVertices.Add(vertices[vertexIndex]);

                physicsShapeGroup2D.AddPolygon(polygonVertices);
                polygonIndex++;
            }

            vertexIndex++;
            angle -= _angleIncrease;
        }

        _collider.SetCustomShapes(physicsShapeGroup2D);

        _mesh.vertices = vertices;
        _mesh.uv = uv;
        _mesh.triangles = triangles;
    }

    public void SetOrigin(Vector3 origin)
    {
        //this._origin = origin;
    }

    public void SetAimDirection(Vector3 aimDirection)
    {
        _startingAngle = GetAngleFromVectorFloat(aimDirection) + _fov / 2f;
    }


    public Vector3 GetVectorFromAngle(float angle)
    {
        float angleRad = angle * (Mathf.PI / 180f);
        return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
    }

    public float GetAngleFromVectorFloat(Vector3 dir)
    {
        dir = dir.normalized;
        float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (n < 0) n += 360;
        return n;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        ResonanceObject resonanceObj = collision.GetComponent<ResonanceObject>();
        if (resonanceObj != null && resonanceObj != _currentResonanceObject)
        {
            _currentResonanceObject = resonanceObj;
            Debug.Log(resonanceObj.Signature.GetSignatureString());
            onSignatureDetected(resonanceObj.Signature);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        ResonanceObject resonanceObj = collision.GetComponent<ResonanceObject>();
        if (resonanceObj != null && resonanceObj == _currentResonanceObject)
        {
            _currentResonanceObject = null;
            onSignatureEmpty();
        }
    }
}
