using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class CollisionEffect : MonoBehaviour
{
    public List<UnityEvent> collisionEvents;

    private Collider2D _collider;

    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (_collider.isTrigger)
            return;

        foreach (var collisionEvent in collisionEvents)
        {
            collisionEvent?.Invoke();
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (!_collider.isTrigger)
            return;

        foreach (var collisionEvent in collisionEvents)
        {
            collisionEvent?.Invoke();
        }
    }
}
