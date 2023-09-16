using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;

[RequireComponent(typeof(Grid))]
public class WindGrid : MonoBehaviour
{
    public TextMeshProUGUI _playerText;
    public List<Wind_Base> windSystems;
    
    private Grid _gridUtil;
    private Character _player;

    public int width;
    public int height;
    private float _cellWidth;
    private float _cellHeight;

    private Dictionary<Vector2Int, WindTile> _windTiles;
    private List<IWindAgent> _windAgents = new List<IWindAgent>();

    private void OnEnable()
    {
        IWindAgent.newAgentSubscribeRequest += SubscribeWindAgent;
    }

    private void OnDisable()
    {
        IWindAgent.newAgentSubscribeRequest += UnsubscribeWindAgent;
    }

    private void Awake()
    {
        _gridUtil = GetComponent<Grid>();
        _windTiles = new Dictionary<Vector2Int, WindTile>();
        _cellWidth = _gridUtil.cellSize.x;
        _cellHeight = _gridUtil.cellSize.y;

        GenerateWindTiles();
        //DrawGridDebug();
    }

    private void Start()
    {
        foreach (var system in windSystems)
        {
            system.Initialize(this);
        }
    }

    private void GenerateWindTiles()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2Int tileCoord = new Vector2Int(x - width / 2, y - height / 2);
                WindTile tile = new WindTile();
                //tile.AddWindForce(new WindForce(2f, Vector2.right));
                _windTiles.Add(tileCoord, tile);
            }
        }
    }

    private void DrawGridDebug()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2Int tileCoord = new Vector2Int(x - width / 2, y - height / 2);
                WindTile tile = _windTiles[tileCoord];

                if (tile != null)
                {
                    Vector3 tileCenter = _gridUtil.GetCellCenterWorld((Vector3Int)tileCoord);
                    Debug.DrawLine(tileCenter, tileCenter + (Vector3)Vector2.up * 0.25f, Color.red, 5f);
                }
            }
        }
    }

    private void Update()
    {
        if (_player != null && _playerText != null)
            _playerText.text = _gridUtil.WorldToCell(_player.transform.position).ToString();

        GenerateWindData();
        SendWindUpdates();
    }

    private void GenerateWindData()
    {
        foreach (var tile in _windTiles)
        {
            tile.Value.ClearWindForces();
        }

        foreach (var wind in windSystems)
        {
            List<WindForce> newWindForces = wind.GenerateWindUpdate();
            foreach (var windForce in newWindForces)
            {
                WindTile tile = null;
                _windTiles.TryGetValue(windForce.cellCoords, out tile);
                tile?.AddWindForce(windForce);
            }
        }
    }

    private void SendWindUpdates()
    {
        Vector2Int agentPos = new Vector2Int();
        foreach (var agent in _windAgents)
        {
            agentPos = (Vector2Int)_gridUtil.WorldToCell(agent.GetWorldPosition());
            WindTile windTile = _windTiles[agentPos];

            if (windTile == null)
                continue;

            agent.WindUpdate(windTile.WindForces);
        }
    }

    public Vector3Int WorldToCell(Vector3 worldPosition)
    {
        return _gridUtil.WorldToCell(worldPosition);
    }

    public Vector3 CellToWorld(Vector3Int cellPosition)
    {
        return _gridUtil.CellToWorld(cellPosition);
    }

    public Vector3 GetCellCenterWorld(Vector3Int cellPosition)
    {
        return _gridUtil.GetCellCenterWorld(cellPosition);
    }

    private void SubscribeWindAgent(IWindAgent agent)
    {
        _windAgents.Add(agent);
    }

    private void UnsubscribeWindAgent(IWindAgent agent)
    {
        _windAgents.Remove(agent);
    }

    private void OnDrawGizmosSelected()
    {
        Grid grid = GetComponent<Grid>();
        Gizmos.DrawCube(Vector2.zero, new Vector3(width * grid.cellSize.x, height * grid.cellSize.y, 1f));
    }
}
