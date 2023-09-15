using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class DebugCharacter : Character
{
    public Action OnJumpRequest;

    // Components
    private Rigidbody2D _rigidbody;

    // Movement
    [SerializeField] private float _moveSpeed = 10f;
    private Vector2 _dir;


    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        UpdateCharacterMovement();
    }

    public override void RequestMove(Vector2 moveDirection)
    {
        _dir = moveDirection;
    }

    private void UpdateCharacterMovement()
    {
        //_dir.Normalize();
        _rigidbody.MovePosition((Vector2)transform.position + (_dir * _moveSpeed * Time.fixedDeltaTime));
    }

}
