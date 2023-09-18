using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class Interactable : MonoBehaviour
{
    public Action<Interactable> onEnable;
    public Action<Interactable> onDisable;

    protected Collider2D _collider;
    protected bool _isEnabled;
    public bool IsEnabled { get { return _isEnabled; } }

    [SerializeField] protected List<UnityEvent> onInteractEvents;
    [SerializeField] protected bool _autoEnable = true;

    protected virtual void Awake()
    {
        _collider = GetComponent<Collider2D>();

        if (_autoEnable)
            Activate();
        else
            Deactivate();
    }

    public void Interact()
    {
        if (!_isEnabled)
            return;

        Debug.Log("Interaction triggered on : " + this.gameObject.name);
        foreach (var interactEvent in onInteractEvents)
        {
            interactEvent?.Invoke();
        }
    }

    public virtual void Activate()
    {
        _isEnabled = true;
        onEnable?.Invoke(this);
    }

    public virtual void Deactivate()
    {
        onDisable?.Invoke(this);
        _isEnabled = false;
    }
}
