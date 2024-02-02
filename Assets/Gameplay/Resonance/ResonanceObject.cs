using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResonanceObject : MonoBehaviour
{
    [SerializeField] private Signature _signature;
    public Signature Signature { get { return _signature; } }
}
