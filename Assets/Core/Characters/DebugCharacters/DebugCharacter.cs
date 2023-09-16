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
    private SpriteRenderer _spriteRenderer;
    private Animator _animator;

    // Movement
    [SerializeField] private float _moveSpeed = 10f;
    private Vector2 _dir;


    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        if (_spriteRenderer == null)
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        if (_animator == null)
            _animator = GetComponentInChildren<Animator>();
    }

    private void FixedUpdate()
    {
        UpdateCharacterMovement();
    }

    public override void RequestMove(Vector2 moveDirection)
    {
        _dir = moveDirection;
        if (_animator != null)
        {
            float animSpeed = moveDirection.magnitude;
            _animator.SetFloat("Speed", animSpeed);

            if (Mathf.Abs(moveDirection.x) > 0.01f)
            {
                _spriteRenderer.flipX = Mathf.Sign(moveDirection.x) < 0;
                Vector2 flip = Vector2.one;
                flip.x = _spriteRenderer.flipX ? -1f : 1f;
                OnDirectionFlip?.Invoke(flip);
            }
        }
    }

    private void UpdateCharacterMovement()
    {
        //_dir.Normalize();
        _rigidbody.MovePosition((Vector2)transform.position + (_dir * _moveSpeed * Time.fixedDeltaTime));
    }

}
