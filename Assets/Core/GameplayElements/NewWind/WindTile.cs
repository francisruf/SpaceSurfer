using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindTile
{
    private List<WindForce> _windForces = new List<WindForce>();
    public List<WindForce> WindForces { get { return _windForces; } }

    public void AddWindForce(WindForce windForce)
    {
        _windForces.Add(windForce);
    }

    public void ClearWindForces()
    {
        _windForces.Clear();
    }
}
