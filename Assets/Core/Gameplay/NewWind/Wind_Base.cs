using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Wind_Base : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] protected bool _debug;

    protected bool _isEnabled;
    protected WindGrid _windGrid;
    protected List<WindForce> _windForces = new List<WindForce>();

    public virtual void Initialize(WindGrid grid)
    {
        _windGrid = grid;
        EnableSystem();
    }

    protected virtual void EnableSystem()
    {
        _isEnabled = true;
    }

    protected virtual void DisableSystem()
    {
        _isEnabled = false;
    }
    public abstract List<WindForce> GenerateWindUpdate();
}
