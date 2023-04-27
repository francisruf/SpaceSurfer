using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGridActor<T>
{
    public void AssignCurrentTileData(T tileData);
    public Vector3 GetWorldPosition();
}
