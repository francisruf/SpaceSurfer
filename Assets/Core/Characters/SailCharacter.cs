using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class SailCharacter : Character
{
    [Header("Prefabs")]
    [SerializeField] private GameObject _defaultSailPrefab;

    // Components
    private Rigidbody2D _rigidbody;

    [Header("Movement")]
    [SerializeField] float _baseMoveSpeed = 5f;
    [SerializeField] float _rotationSpeed = 5f;

    // Movement logic
    private Vector2 _targetDirection = Vector2.up;
    private Vector2 _targetPosition;
    private Vector2 _baseMoveVector;
    private Vector2 _sailMoveVector;
    private Vector2[] _modifiersMoveVector = new Vector2[10];

    // Sail logic
    private Sail _currentSail;

    [Header("Debug")]
    [SerializeField] private bool _addBaseMovement = false;
    [SerializeField] private Vector2[] _debugForces = new Vector2[10];

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    protected override void Start()
    {
        base.Start();

        // TODO : Temporary
        SpawnDefaultSail();
    }

    private void Update()
    {
        if (_debugEnabled)
            DrawTargetRotation();
    }

    private void FixedUpdate()
    {
        AssignBaseMovementForce();
        AssignSailMovementForce();
        AssignDebugForces();

        RotateCharacter();
        MoveCharacter();
    }

    public override void RequestMove(Vector2 direction)
    {
        if (direction == Vector2.zero)
            return;

        _targetDirection = direction;
    }

    private void RotateCharacter()
    {
        float targetAngle = (Mathf.Atan2(_targetDirection.y, _targetDirection.x) * Mathf.Rad2Deg) - 90f;
        float newAngle = Mathf.LerpAngle(transform.rotation.eulerAngles.z, targetAngle, _rotationSpeed * Time.fixedDeltaTime);
        _rigidbody.MoveRotation(newAngle);
    }

    private void AssignBaseMovementForce()
    {
        if (_addBaseMovement)
            _modifiersMoveVector[0] = transform.up * _baseMoveSpeed * Time.fixedDeltaTime;
    }

    private void AssignSailMovementForce()
    {
        if (_currentSail == null)
            return;

        _sailMoveVector = _currentSail.GetMoveVector();
    }

    private void AssignDebugForces()
    {
        for (int i = 0; i < 10; i++)
        {
            _modifiersMoveVector[i] = _debugForces[i] * Time.fixedDeltaTime;
        }
    }

    // TODO : Refactor this with forces
    private void MoveCharacter()
    {
        _targetPosition = transform.position;
        _targetPosition += _baseMoveVector;
        _targetPosition += _sailMoveVector;
        _baseMoveVector = Vector2.zero;
        _sailMoveVector = Vector2.zero;

        for (int i = 0; i < 10; i++)
        {
            _targetPosition += _modifiersMoveVector[i];
            _modifiersMoveVector[i] = Vector2.zero;
        }

        _rigidbody.MovePosition(_targetPosition);
    }

    private void DrawTargetRotation()
    {
        Debug.DrawLine(transform.position, transform.position + (Vector3)_targetDirection, Color.red);
        Debug.DrawLine(transform.position, transform.position + transform.up, Color.green);
    }

    private void SpawnDefaultSail()
    {
        Sail defaultSail = Instantiate(_defaultSailPrefab, transform).GetComponent<Sail>();
        AssignNewSail(defaultSail);
    }

    private void AssignNewSail(Sail newSail)
    {
        _currentSail = newSail;
        _currentSail?.ChangeSailState(ESailState.Closed);
    }

    public void RequestSailOpen()
    {
        if (_currentSail == null)
            return;

        int newState = Mathf.Clamp(((int)_currentSail.SailState) + 1, 0, 2);
        _currentSail.ChangeSailState((ESailState)newState);
    }

    public void RequestSailClose()
    {
        if (_currentSail == null)
            return;

        int newState = Mathf.Clamp(((int)_currentSail.SailState) - 1, 0, 2);
        _currentSail.ChangeSailState((ESailState)newState);
    }
}
