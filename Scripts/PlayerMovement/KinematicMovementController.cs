using UnityEngine;
using System.Runtime.CompilerServices;

public sealed class KinematicMovementController : IMovementController
{
    private static Vector3 _tempVelocity;
    private static Vector3 _tempTargetVelocity;
    private static Vector3 _tempHorizontalVelocity;

    private Vector3 _velocity;
    private readonly MovementConfig _config;
    private readonly Transform _transform;

    public Vector3 Velocity => _velocity;

    public KinematicMovementController(MovementConfig config, Transform transform)
    {
        _config = config;
        _transform = transform;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void UpdateMovement(Vector2 input, MovementState state, float deltaTime)
    {
        bool isGrounded = (state & MovementState.Grounded) != 0;
        bool isSprinting = (state & MovementState.Sprinting) != 0;

        float speed = isSprinting ? _config.SprintSpeed : _config.WalkSpeed;

        _tempTargetVelocity.x = input.x * speed;
        _tempTargetVelocity.y = 0;
        _tempTargetVelocity.z = input.y * speed;

        _tempTargetVelocity = _transform.TransformDirection(_tempTargetVelocity);

        if (isGrounded)
        {
            _tempVelocity.x = Mathf.Lerp(_velocity.x, _tempTargetVelocity.x, _config.MovementSharpness * deltaTime);
            _tempVelocity.z = Mathf.Lerp(_velocity.z, _tempTargetVelocity.z, _config.MovementSharpness * deltaTime);
            _tempVelocity.y = _velocity.y;
            _velocity = _tempVelocity;
        }
        else
        {
            _tempVelocity = _tempTargetVelocity * (_config.AccelerationInAir * deltaTime);
            _velocity.x += _tempVelocity.x;
            _velocity.z += _tempVelocity.z;

            _tempHorizontalVelocity.x = _velocity.x;
            _tempHorizontalVelocity.y = 0;
            _tempHorizontalVelocity.z = _velocity.z;

            float maxAirSpeed = speed * _config.AirSpeedLimit;
            float horizontalMagnitude = _tempHorizontalVelocity.magnitude;

            if (horizontalMagnitude > maxAirSpeed)
            {
                float factor = maxAirSpeed / horizontalMagnitude;
                _velocity.x *= factor;
                _velocity.z *= factor;
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ApplyGravity(float deltaTime)
    {
        _velocity.y -= _config.Gravity * deltaTime;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Jump(float force)
    {
        _velocity.y = force;
    }
}