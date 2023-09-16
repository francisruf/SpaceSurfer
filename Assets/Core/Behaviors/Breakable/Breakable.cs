using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Animator))]
public class Breakable : MonoBehaviour
{
    public List<Event> onBreakEvents;
    private Animator _animator;
    private Collider2D _collider;
    bool _broken = false;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _collider = GetComponent<Collider2D>();
    }

    public void Break()
    {
        if (!_broken)
        {
            if (_collider != null)
                _collider.enabled = false;

            _animator.SetTrigger("Break");
            _broken = true;
        }
    }
}
