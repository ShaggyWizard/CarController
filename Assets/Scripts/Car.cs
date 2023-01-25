using System;
using System.Collections;
using UnityEngine;

public class Car : MonoBehaviour
{
    [Header("Car Settings")]
    [SerializeField] private float _maxSpeed;
    [SerializeField] private float _timeToMaxSpeed, _maxSpeedModifyer, _timeToModify;
    [SerializeField] private float _timeToStop, _backwardsAcceleration;
    [SerializeField] private float _defaultDeceleration;
    [SerializeField] private float _maxSteerAngle;
    [Header("Dependencies")]
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private WheelCollider _frontLeftWheelCollider, _frontRightWheelCollider;
    [SerializeField] private WheelCollider _rearLeftWheelCollider, _rearRightWheelCollider;


    public event Action<float> OnSpeedChange;
    public float MaxSpeed => _maxSpeed;

    public float Speed { get; private set; }


    private float _maxSpeedDelta;
    private float _targetSpeed;
    private float _lastModifyedSpeedLimit;
    private float _deceleration;
    private bool _lastUpdateForward;
    private float _horizontalInput, _verticalInput;
    private float _currentSteerAngle;
    private bool _inAir => !_rearLeftWheelCollider.isGrounded && !_rearRightWheelCollider.isGrounded;


    private void Awake()
    {
        _maxSpeedDelta = _maxSpeed / _timeToMaxSpeed;
    }
    private void FixedUpdate()
    {
        GetInput();
        CalculateSpeed();
        HandleMotor();
        HandleSteering();
        OnSpeedChange?.Invoke(Speed);
    }


    public void Boost(Vector3 direction)
    {
        var beforeBoost = _targetSpeed;
        _targetSpeed += _targetSpeed * Vector3.Dot(direction, transform.forward.normalized);
        Debug.Log($"Boosted from: {beforeBoost} to: {_targetSpeed} dot: {Vector3.Dot(direction.normalized, transform.forward.normalized)}");
    }
    private void CalculateSpeed()
    {
        if (_verticalInput ==  0)
        {
            _targetSpeed = 0;
        }
        else if (_verticalInput > 0)
        {
            if (_inAir && _targetSpeed > 0)
            {
                return;
            }
            if (Speed >= _maxSpeed)
            {
                var modifyedMaxSpeed = _lastModifyedSpeedLimit * _maxSpeedModifyer;
                var acceleration = (modifyedMaxSpeed - _lastModifyedSpeedLimit) / _timeToModify;
                _targetSpeed += acceleration * _verticalInput * Time.fixedDeltaTime;
                if (_targetSpeed > modifyedMaxSpeed)
                {
                    _lastModifyedSpeedLimit = modifyedMaxSpeed;
                }
            }
            else
            {
                _lastModifyedSpeedLimit = _maxSpeed;
                _targetSpeed = _maxSpeed;
            }
        }
        else
        {
            if (_inAir && _targetSpeed < 0)
            {
                return;
            }
            if (_lastUpdateForward)
            {
                _deceleration = _targetSpeed / _timeToStop;
            }

            if (_targetSpeed > 0)
                _targetSpeed -= _deceleration * Time.fixedDeltaTime;
            else
                _targetSpeed += _backwardsAcceleration * _verticalInput * Time.fixedDeltaTime;

            _lastUpdateForward = false;
        }
    }
    private void GetInput()
    {
        _horizontalInput = Input.GetAxisRaw("Horizontal");

        _verticalInput = Input.GetAxisRaw("Vertical");
    }
    private void HandleMotor()
    {
        if (_inAir)
        {
            return;
        }

        var currentGravity = Vector3.Project(_rigidbody.velocity, -transform.up);
        var currentSpeed = Vector3.Dot(_rigidbody.velocity, transform.forward);
        Speed = Mathf.MoveTowards(currentSpeed, _targetSpeed, _maxSpeedDelta * Time.fixedDeltaTime);
        _rigidbody.velocity = transform.forward * Speed;
        _rigidbody.velocity += currentGravity + Physics.gravity * Time.fixedDeltaTime;
    }
    private void HandleSteering()
    {
        _currentSteerAngle = _maxSteerAngle * _horizontalInput;
        _frontLeftWheelCollider.steerAngle = _currentSteerAngle;
        _frontRightWheelCollider.steerAngle = _currentSteerAngle;
    }
}