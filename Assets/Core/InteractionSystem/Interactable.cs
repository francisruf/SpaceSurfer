using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class Interactable : MonoBehaviour
{
    protected Collider2D _collider;

    [SerializeField] protected List<UnityEvent> onInteractEvents;

    protected virtual void Awake()
    {
        _collider = GetComponent<Collider2D>();
    }

    public void Interact()
    {
        Debug.Log("Interaction triggered on : " + this.gameObject.name);
        foreach (var interactEvent in onInteractEvents)
        {
            interactEvent?.Invoke();
        }
    }
}
