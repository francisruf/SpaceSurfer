using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider2D))]
public class InteractionController : MonoBehaviour
{
    public Collider2D interactionDetector;
    private Character _character;

    private List<Interactable> _availableInteractables = new List<Interactable>();
    private Interactable _activeInteractable;

    private bool _canInteract;
    public bool debug;

    private void Awake()
    {
        if (interactionDetector == null)
            Debug.LogWarning("No interaction detector found on interaction controller.");
    }

    private void OnEnable()
    {
        _character = GetComponentInParent<Character>();
        if (_character == null)
            _character = GetComponent<Character>();
        if (_character != null)
            _character.OnDirectionFlip += FlipCollider;
    }

    private void OnDisable()
    {
        if (_character != null)
            _character.OnDirectionFlip -= FlipCollider;

        foreach (var interactable in _availableInteractables)
        {
            interactable.onDisable -= RemoveInteractable;
        }
    }

    private void Update()
    {
    }

    public void SetCanInteract(bool canInteract)
    {
        _canInteract = canInteract;
    }

    public void OnInteract(InputValue value)   
    {
        if (_activeInteractable == null || !_canInteract)
            return;

        if (debug)
        {
            Debug.Log("Interact : " + _activeInteractable.gameObject.name);
            foreach (var interact in _availableInteractables)
            {
                Color lineColor = interact == _activeInteractable ? Color.green : Color.red;
                Debug.DrawLine(transform.position, interact.gameObject.transform.position, lineColor, 5f);
            }
        }

        _activeInteractable.Interact();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        Interactable interactable = collider.GetComponent<Interactable>();
        if (interactable != null)
        {
            if (interactable.IsEnabled)
                AddInteractable(interactable);
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        Interactable interactable = collider.GetComponent<Interactable>();
        if (interactable != null)
            RemoveInteractable(interactable);
    }

    private void AddInteractable(Interactable interactable)
    {
        _availableInteractables.Add(interactable);
        interactable.onDisable += RemoveInteractable;

        SetInteractPriority();

        if (debug)
            Debug.Log("Interact available : " + interactable.gameObject.name);
    }

    private void RemoveInteractable(Interactable interactable)
    {
        interactable.onDisable -= RemoveInteractable;
        _availableInteractables.Remove(interactable);

        SetInteractPriority();

        if (debug)
            Debug.Log("Interact no longer available : " + interactable.gameObject.name);
    }

    private void FlipCollider(Vector2 direction)
    {
        if (interactionDetector == null)
            return;

        Vector2 newOffset = interactionDetector.offset;
        newOffset.x = Mathf.Abs(newOffset.x) * Mathf.Sign(direction.x);
        newOffset.y = Mathf.Abs(newOffset.y) * Mathf.Sign(direction.y);
        interactionDetector.offset = newOffset;
    }

    public void OnMove(InputValue value)
    {
        SetInteractPriority();
    }

    private void SetInteractPriority()
    {
        int interactCount = _availableInteractables.Count;
        //if (interactCount <= 0)
            //return;

        float closestDistance = float.MaxValue;
        Interactable candidate = null;

        for (int i = 0; i < _availableInteractables.Count; i++)
        {
            float distanceWithManager = Vector2.Distance(_availableInteractables[i].gameObject.transform.position, transform.position);
            if (distanceWithManager < closestDistance)
            {
                candidate = _availableInteractables[i];
                closestDistance = distanceWithManager;
            }
        }
        _activeInteractable = candidate;
    }
}
