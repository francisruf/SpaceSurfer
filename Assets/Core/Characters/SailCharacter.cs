using System;
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
public class SailCharacter : Character, IImpactable
{
    public static Action<SailCharacter> onSailCharacterExplode;

    [Header("Prefabs")]
    [SerializeField] private GameObject _defaultSailPrefab;

    [Header("Components")]
    private Animator _boardAnimator;
    private Rigidbody2D _rigidbody;
    [SerializeField] private SpriteRenderer[] _spriteRenderers;
    private Collider2D _collider;

    [Header("Movement")]
    [SerializeField] float _baseMoveSpeed = 5f;
    [SerializeField] float _rotationSpeed = 5f;
    [SerializeField] AnimationCurve _accelerationCurve;
    [SerializeField] float _maxSpeedPerSecond = 10f;
    [SerializeField] private float _baseAccelerationRate = 5f;
    [SerializeField] private float _baseDecelerationRate = 5f;
    [SerializeField] private float _baseBrakingRate = 1f;

    // Movement logic
    private Vector2 _targetDirection = Vector2.up;
    private Vector2 _targetPosition;
    private Vector2 _sailMoveVector;
    private Vector3 _windDisplacement;
    private Vector3 _targetWindDisplacement;
    private float _acceleration;
    private float _braking;
    private float _brakingAlpha;
    private Vector2[] _modifiers = new Vector2[5];


    [Header("Sail")]
    [SerializeField] private ESailRotationType _sailRotationType;
    [SerializeField] private bool _clampSailRotation;
    [SerializeField] private float _sailRotationSpeed = 5f;
    [SerializeField] private float _sailPressureSpeed = 2f;
    [Range(0f, 180f)]
    [SerializeField] private float _maxSailAngle = 85f;
    [SerializeField] private Transform _sailAttachPoint;

    private Sail _currentSail;
    private float _targetSailAngle;
    private float _targetSailDirection;
    private float _currentSailPressure;
    private float _targetSailPressure;

    [Header("Impacts")]
    [SerializeField] private AnimationCurve _impactCurve;
    [SerializeField] private float _impactDurationModifier = 1f;
    [SerializeField] private float _impactPeakModifier = 1f;
    [Range(0f, 1f)]
    [SerializeField] private float _impactMaxVelocityLoss = 1f;
    [SerializeField] private float _impactMaxVelocityLossThreshold = 2f;
    [SerializeField] private float _impactFatalThreshold = 3f;
    [SerializeField] private GameObject _superLol;


    private void Awake()
    {
        _boardAnimator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
    }

    protected override void Start()
    {
        base.Start();
    }

    private void Update()
    {
        if (_debugEnabled)
            DrawTargetRotation();
    }

    private void FixedUpdate()
    {
        RotateCharacter();
        RotateSail();
        UpdateSailPressure();
        AssignWindForce();
        AssignGlobalVelocity();
    }

    public override void SetDebugEnabled(bool enableDebug)
    {
        base.SetDebugEnabled(enableDebug);

        if (_currentSail != null)
            _currentSail.SetDebugEnabled(_debugEnabled);
    }

    public override void RequestMove(Vector2 direction)
    {
        if (direction == Vector2.zero)
            return;

        _targetDirection = direction;
    }

    public void RequestBraking(float brakingAlpha)
    {
        _brakingAlpha = brakingAlpha;
        _boardAnimator.SetFloat("BrakingAlpha", _brakingAlpha);
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
                    _targetSailAngle = Mathf.Clamp(_targetSailAngle, -_maxSailAngle, _maxSailAngle);
                break;
            default:
                break;
        }
        _currentSail.transform.localRotation = Quaternion.Euler(0f, 0f, _targetSailAngle);
    }

    private void AssignWindForce()
    {
        if (_currentSail == null)
            return;

        _sailMoveVector = _currentSail.GetMoveVector();
        float windForce = _sailMoveVector.magnitude;
        windForce += windForce * _currentSailPressure / 2f;

        _acceleration = _baseAccelerationRate * windForce * _accelerationCurve.Evaluate(GetSafeAlpha(_windDisplacement.magnitude, windForce));
        _acceleration += _acceleration * _currentSailPressure * 3f;

        // Get a little more braking the faster you go, but clamp min to avoid low braking rate when going slow.
        _braking = _brakingAlpha * (_baseBrakingRate + (_baseBrakingRate * _windDisplacement.magnitude / 2f));

        if (windForce > _targetWindDisplacement.magnitude)
        {
            _targetWindDisplacement = transform.up * windForce;
        }

        else
        {
            _targetWindDisplacement = transform.up * Mathf.Clamp(_targetWindDisplacement.magnitude - 
                (_targetWindDisplacement.magnitude * _baseDecelerationRate * Time.fixedDeltaTime), 0f, _maxSpeedPerSecond);
        }

        _windDisplacement = transform.up * Mathf.Clamp(_windDisplacement.magnitude + (_acceleration * Time.fixedDeltaTime) - (_braking * Time.fixedDeltaTime), 0f, _targetWindDisplacement.magnitude);
    }

    private int GetNextModifierSlot(out bool success)
    {
        int index = 0;
        success = false;
        for (int i = 0; i < _modifiers.Length; i++)
        {
            if (Mathf.Approximately(_modifiers[i].magnitude, 0f))
            {
                index = i;
                success = true;
                break;
            }
        }
        return index;
    }

    private IEnumerator AddModifierForce(float duration, float peakForce, Vector2 direction, AnimationCurve modifierCurve)
    {
        bool freeModifierSlot = false;
        int index = GetNextModifierSlot(out freeModifierSlot);

        if (!freeModifierSlot)
            yield return null;
        
        else
        {
            float time = 0f;
            
            while(time < duration)
            {
                _modifiers[index] = direction.normalized * (modifierCurve.Evaluate(time / duration) * peakForce * Time.fixedDeltaTime);
                time += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }

            _modifiers[index] = Vector2.zero;
        }
    }

    private void AssignGlobalVelocity()
    {
        _targetPosition = transform.position + _windDisplacement * Time.fixedDeltaTime;
        
        foreach (var modifier in _modifiers)
            _targetPosition += modifier;

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
        Transform spawnTransform = _sailAttachPoint == null ? transform : _sailAttachPoint;
        Sail defaultSail = Instantiate(_defaultSailPrefab, spawnTransform).GetComponent<Sail>();
        AssignNewSail(defaultSail);
    }

    private void AssignNewSail(Sail newSail)
    {
        _currentSail = newSail;
        _currentSail.ChangeSailState(ESailState.Closed);
    }

    public void RequestSailOpen()
    {
        if (_currentSail == null)
            return;

        int newState = Mathf.Clamp(((int)_currentSail.SailState) + 1, 0, 1);
        _currentSail.ChangeSailState((ESailState)newState);
    }

    public void RequestSailClose()
    {
        if (_currentSail == null)
            return;

        int newState = Mathf.Clamp(((int)_currentSail.SailState) - 1, 0, 1);
        _currentSail.ChangeSailState((ESailState)newState);
    }

    public void RequestSailPressure(float inputValue)
    {
        _targetSailPressure = inputValue;
    }

    private void UpdateSailPressure()
    {
        if (_currentSail == null)
            return;

        float filteredPressure = _targetSailPressure;

        if (_currentSail.SailState == ESailState.Closed)
            filteredPressure = 0f;

        float directionModifier = filteredPressure - _currentSailPressure;
        _currentSailPressure += _sailPressureSpeed * directionModifier * Time.deltaTime;

        // TODO : Why the F won't a Mathf.Round work
        if (_currentSailPressure < 0.01f)
            _currentSailPressure = 0f;

        else if (_currentSailPressure > 0.99f)
            _currentSailPressure = 1f;

        _currentSail.SetSailPressure(_currentSailPressure);
    }

    public void Impact(Vector2 impact, Vector2 impactPoint)
    {
        float magnitude = impact.magnitude;

        if (magnitude >= _impactFatalThreshold)
        {
            Explode(impact, impactPoint);
            return;
        }

        float duration = _impactDurationModifier * magnitude;
        float peakForce = _impactPeakModifier * magnitude;

        float velocityLossPercent = Mathf.InverseLerp(0f, _impactMaxVelocityLossThreshold, magnitude) * _impactMaxVelocityLoss;
        velocityLossPercent = Mathf.Clamp(velocityLossPercent, 0f, _impactMaxVelocityLoss);
        _windDisplacement -= _windDisplacement * velocityLossPercent;

        DebugManager.instance.RequestNotification("Impact : " + magnitude.ToString("F3") +
            "\nPeak force : " + peakForce.ToString("F3") +
            "\nVelocity loss : " + (int)(velocityLossPercent * 100f) + "%" +
            "\nDuration : " + duration.ToString("F3"));

        StartCoroutine(AddModifierForce(duration, peakForce, impact, _impactCurve));
    }

    private void Explode(Vector2 impact, Vector2 impactPoint)
    {
        DebugManager.instance.RequestNotification   ("You died lmao" + "\nImpact : " + impact.magnitude.ToString("F3"));
        onSailCharacterExplode?.Invoke(this);
        DisableCharacter();

        if (_superLol != null)
        {
            GameObject explotion = Instantiate(_superLol, impactPoint, Quaternion.identity);
        }
    }

    public override string GetCharacterDebugData()
    {
        string characterData = "<size=150%>Sail Character</size>";
        characterData += "\nVelocity : " + _windDisplacement.magnitude.ToString("F3");
        characterData += "\nTarget velocity " + _targetWindDisplacement.magnitude.ToString("F3");
        characterData += "\nAccel curve : " + Mathf.Clamp(GetSafeAlpha(_windDisplacement.magnitude, _targetWindDisplacement.magnitude), 0f, 1f).ToString("F3");
        characterData += "\nSail force : " + _sailMoveVector.magnitude.ToString("F3");
        characterData += "\nAcceleration : " + _acceleration.ToString("F3");
        characterData += "\nBraking : " + _braking.ToString("F3");
        characterData += "\nTarget Sail Angle : " + _targetSailAngle.ToString("F3");
        return characterData;
    }

   

    public override void EnableCharacter(bool isPlayerCharacter)
    {
        foreach (var renderer in _spriteRenderers)
            renderer.enabled = true;

        _collider.enabled = true;
        _rigidbody.isKinematic = false;

        if (_currentSail == null)
            SpawnDefaultSail();

        else
            _currentSail.EnableSail();

        base.EnableCharacter(isPlayerCharacter);
    }

    public override void DisableCharacter()
    {
        if (_currentSail != null)
            _currentSail.DisableSail();

        ResetMovementValues();

        foreach (var renderer in _spriteRenderers)
            renderer.enabled = false;

        _collider.enabled = false;
        _rigidbody.isKinematic = true;
        base.DisableCharacter();
    }

    private void ResetMovementValues()
    {
        _targetDirection = Vector2.up;
        _targetPosition = Vector2.zero;
        _sailMoveVector = Vector2.zero;
        _windDisplacement = Vector3.zero;
        _targetWindDisplacement = Vector3.zero;
        _acceleration = 0f;
        for (int i = 0; i < _modifiers.Length; i++)
            _modifiers[i] = Vector2.zero;
        _targetSailAngle = 0f;
        _targetSailDirection = 0f;
        _currentSailPressure = 0f;
        _targetSailPressure = 0f;
        _brakingAlpha = 0f;
        _braking = 0f;
        _boardAnimator.SetFloat("BrakingAlpha", 0f);
    }
}
