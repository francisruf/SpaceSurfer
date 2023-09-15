using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crystal : MonoBehaviour
{
    private Animator _animator;
    private Collider2D _collider;
    bool _destroyed = false;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _collider = GetComponent<Collider2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Break();
    }

    private void Break()
    {
        if (!_destroyed)
        {
            _collider.enabled = false;
            _animator.SetTrigger("Break");
            _destroyed = true;
        }

    }

}
