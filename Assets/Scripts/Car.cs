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
    [SerializeField] private Transform _frontLeftWheelTransform, _frontRightWheelTransform;
    [SerializeField] private Transform _rearLeftWheelTransform, _rearRightWheelTransform;


    public event Action<float> OnSpeedChange;
    public float MaxSpeed => _maxSpeed;
    public float Speed { get; private set; }


    private float _lastModifyedSpeedLimit;
    private float _deceleration;
    private bool _lastUpdateForward;
    private bool _lastUpdateInAir;
    private float _horizontalInput, _verticalInput;
    private float _currentSteerAngle;
    private bool _inAir => !_rearLeftWheelCollider.isGrounded && !_rearRightWheelCollider.isGrounded;

    private void FixedUpdate()
    {
        GetInput();
        CalculateSpeed();
        HandleMotor();
        HandleSteering();
        UpdateWheels();
        OnSpeedChange?.Invoke(Speed);
        if (_inAir)
            _lastUpdateInAir = true;
    }

    
    public void Boost(Vector3 direction)
    {
        Speed += Speed * Vector3.Dot(direction, transform.forward.normalized);
    }


    private void CalculateSpeed()
    {
        if (_lastUpdateInAir && _rearLeftWheelCollider.isGrounded && _rearRightWheelCollider.isGrounded)
        {
            Speed = Vector3.Scale(transform.forward, _rigidbody.velocity).magnitude;
        }
        if (_verticalInput == 0)
        {
            _lastUpdateForward = true;
            if (Speed > 0)
                Speed = Mathf.Max(0, Speed - _defaultDeceleration * Time.fixedDeltaTime);
            else if (Speed < 0)
                Speed = Mathf.Min(0, Speed + _defaultDeceleration * Time.fixedDeltaTime);
        }
        else if (_verticalInput > 0)
        {
            if (_inAir && Speed > 0)
            {
                return;
            }
            _lastUpdateForward = true;
            var acceleration = _maxSpeed / _timeToMaxSpeed;
            var modifyedMaxSpeed = _lastModifyedSpeedLimit * _maxSpeedModifyer;

            if (Speed > _maxSpeed)
            {
                acceleration = (modifyedMaxSpeed - _lastModifyedSpeedLimit) / _timeToModify;
            }
            else
            {
                _lastModifyedSpeedLimit = _maxSpeed;
                modifyedMaxSpeed = _lastModifyedSpeedLimit * _maxSpeedModifyer;
            }

            Speed += acceleration * _verticalInput * Time.fixedDeltaTime;

            //step up
            if (Speed > modifyedMaxSpeed)
            {
                _lastModifyedSpeedLimit = modifyedMaxSpeed;
            }
        }
        else
        {
            if (_inAir && Speed < 0)
            {
                return;
            }
            if (_lastUpdateForward)
            {
                _deceleration = Speed / _timeToStop;
            }

            if (Speed > 0)
                Speed -= _deceleration * Time.fixedDeltaTime;
            else
                Speed += _backwardsAcceleration * _verticalInput * Time.fixedDeltaTime;

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

        _rigidbody.velocity = transform.forward * Speed;
        _rigidbody.velocity += Physics.gravity * Time.fixedDeltaTime;
    }
    private void HandleSteering()
    {
        _currentSteerAngle = _maxSteerAngle * _horizontalInput;
        _frontLeftWheelCollider.steerAngle = _currentSteerAngle;
        _frontRightWheelCollider.steerAngle = _currentSteerAngle;
    }
    private void UpdateWheels()
    {
        UpdateSingleWheel(_frontLeftWheelCollider, _frontLeftWheelTransform);
        UpdateSingleWheel(_frontRightWheelCollider, _frontRightWheelTransform);
        UpdateSingleWheel(_rearRightWheelCollider, _rearRightWheelTransform);
        UpdateSingleWheel(_rearLeftWheelCollider, _rearLeftWheelTransform);
    }
    private void UpdateSingleWheel(WheelCollider wheelCollider, Transform wheelTransform)
    {
        Vector3 pos;
        Quaternion rot;
        wheelCollider.GetWorldPose(out pos, out rot);
        wheelTransform.rotation = rot;
        wheelTransform.position = pos;
    }
}