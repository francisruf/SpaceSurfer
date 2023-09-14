using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(DebugCharacter))]
public class DebugCharacterController : PlayerController
{
    private DebugCharacter _character;
    private SpriteRenderer _spriteRenderer;
    private Animator _animator;
    private Rigidbody2D _rigidbody;


    protected override void Awake()
    {
        base.Awake();
        _character = GetComponent<DebugCharacter>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        if (_spriteRenderer == null)
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        if (_animator == null)
            _animator = GetComponentInChildren<Animator>();
    }

    protected virtual void Update()
    {

    }

    public override void OnMove(InputValue value)
    {
        Vector2 input = value.Get<Vector2>();

        _character?.RequestMove(input);
        if (_animator != null)
        {
            float animSpeed = input.magnitude;
            _animator.SetFloat("Speed", animSpeed);


            if (animSpeed > 0.01f)
                _spriteRenderer.flipX = Mathf.Sign(input.x) < 0;
        }
    }
}
