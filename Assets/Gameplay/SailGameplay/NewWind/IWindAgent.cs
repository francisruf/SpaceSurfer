using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWindAgent
{
    public static Action<IWindAgent> newAgentSubscribeRequest;
    public static Action<IWindAgent> newAgentUnubscribeRequest;

    public void WindUpdate(List<WindForce> windForces);
    public Vector3 GetWorldPosition();
}
