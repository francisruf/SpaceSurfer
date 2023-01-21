using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public abstract class PlayerController : MonoBehaviour
{
    public static Action<PlayerController> OnPlayerControllerInstantiate;

    protected virtual void Start()
    {
        if (OnPlayerControllerInstantiate != null)
            OnPlayerControllerInstantiate(this);
    }

    public abstract void OnMove(InputValue value);
}
