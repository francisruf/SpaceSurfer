using UnityEngine;

public struct WindForce
{
    public Vector2Int cellCoords;
    public float strength;
    public Vector2 direction;

    public WindForce(Vector2Int cellCoords, float strength, Vector2 direction)
    {
        this.cellCoords = cellCoords;
        this.strength = strength;
        this.direction = direction;
    }
}
