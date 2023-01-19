using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerController : MonoBehaviour
{
    public static Action<PlayerController> OnPlayerControllerInstantiate;
    public static Action<PlayerController> OnWorldPositionUpdate;

    // Params
    [SerializeField] private float _moveSpeed;

    // Components
    private Rigidbody2D _rb;

    // Movement variables
    private Vector2 _dir = new Vector2();

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        if (OnPlayerControllerInstantiate != null)
            OnPlayerControllerInstantiate(this);
        
        StartCoroutine(UpdateWorldPosition());
    }

    private void FixedUpdate()
    {
        UpdateCharacterMovement();
    }

    private void UpdateCharacterMovement()
    {
        _dir.x = Input.GetAxis("Horizontal");
        _dir.y = Input.GetAxis("Vertical");

        _dir.Normalize();
        _rb.MovePosition((Vector2)transform.position + (_dir * _moveSpeed * Time.fixedDeltaTime));
    }

    // Send player world position update 10 times per sec
    private IEnumerator UpdateWorldPosition()
    {
        while (true)
        {
            if (OnWorldPositionUpdate != null)
                OnWorldPositionUpdate(this);

            yield return new WaitForSeconds(0.1f);
        }
    }
}
