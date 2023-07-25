using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO : TEMP DEBUG
public enum ESailRotationType
{
    Passthrough,
    OneDimension
}

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
    [SerializeField] AnimationCurve _accelerationCurve;
    [SerializeField] float _maxSpeedPerSecond = 10f;


    // Movement logic
    private Vector2 _targetDirection = Vector2.up;
    private Vector2 _targetPosition;
    private Vector2 _sailMoveVector;
    private float _velocity;
    private float _targetVelocity;
    private float _acceleration;
    [SerializeField] private float _baseAccelerationRate = 5f;
    [SerializeField] private float _baseDecelerationRate = 5f;

    // Sail logic
    private Sail _currentSail;
    private float _targetSailAngle;
    private float _targetSailDirection;

    [Header("Debug")]
    [SerializeField] private ESailRotationType _sailRotationType;
    [SerializeField] private bool _clampSailRotation;
    [SerializeField] private float _sailRotationSpeed = 5f;

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
        AssignSailMovementForce();
        RotateCharacter();
        RotateSail();
        AssignVelocity();
    }

    public override void ToggleDebug()
    {
        base.ToggleDebug();
        _currentSail?.ToggleDebug(_debugEnabled);
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

    public virtual void RequestSailRotation(Vector2 direction)
    {
        if (_sailRotationType != ESailRotationType.Passthrough)
            return;

        //TODO : Hard coded deadzone
        if (direction.magnitude > 0.5f)
        {
            direction.Normalize();
            _targetSailAngle = (Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg) - 90f;
        }
    }

    public virtual void RequestSailRotation1D(float direction)
    {
        if (_sailRotationType != ESailRotationType.OneDimension)
            return;

        _targetSailDirection = -direction;

    }

    private void RotateSail()
    {
        switch (_sailRotationType)
        {
            case ESailRotationType.OneDimension:
                _targetSailAngle += (_targetSailDirection * _sailRotationSpeed * Time.fixedDeltaTime);
                if (_clampSailRotation)
                    _targetSailAngle = Mathf.Clamp(_targetSailAngle, -85f, 85f);
                break;
            default:
                break;
        }
        _currentSail.transform.localRotation = Quaternion.Euler(0f, 0f, _targetSailAngle);
    }

    private void AssignSailMovementForce()
    {
        if (_currentSail == null)
            return;

        _sailMoveVector = _currentSail.GetMoveVector();
    }

    private void AssignVelocity()
    {
        // Fuck off


        float windForce = _sailMoveVector.magnitude;

        _acceleration = _baseAccelerationRate * windForce * _accelerationCurve.Evaluate(GetSafeAlpha(_velocity, windForce));

        if (windForce > _targetVelocity)
            _targetVelocity = windForce;

        else
            _targetVelocity = Mathf.Clamp(_targetVelocity - (_targetVelocity / _baseDecelerationRate * Time.fixedDeltaTime), 0f, _maxSpeedPerSecond);

        _velocity = Mathf.Clamp(_velocity + (_acceleration * Time.fixedDeltaTime), 0f, _targetVelocity);

        _targetPosition = transform.position + transform.up * _velocity * Time.fixedDeltaTime;
        _rigidbody.MovePosition(_targetPosition);
    }

    private float GetSafeAlpha(float a, float b)
    {
        if (Mathf.Approximately(b, 0f))
            return 0f;

        else return a / b;
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

    public override string GetCharacterDebugData()
    {
        string characterData = "<size=150%>Sail Character</size>";
        characterData += "\nVelocity : " + _velocity.ToString("F3");
        characterData += "\nTarget velocity " + _targetVelocity.ToString("F3");
        characterData += "\nAccel curve : " + Mathf.Clamp(GetSafeAlpha(_velocity, _targetVelocity), 0f, 1f).ToString("F3");
        characterData += "\nSail force : " + _sailMoveVector.magnitude.ToString("F3");
        characterData += "\nAcceleration : " + _acceleration.ToString("F3");
        characterData += "\nTarget Sail Angle : " + _targetSailAngle;
        return characterData;
    }
}
