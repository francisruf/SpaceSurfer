using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Sail", menuName = "ScriptableObjects/Sail Properties")]
public class SailProperties : ScriptableObject
{
    public float closedSpeed;
    public float raisedSpeed;
    public float extendedSpeed;
}
