using UnityEngine;
using System.Runtime.CompilerServices;
using System;

[RequireComponent(typeof(CharacterController))]
public sealed class OptimizedFpsMovement : MonoBehaviour
{
    [SerializeField] private MovementConfig _config;
    [SerializeField] private Transform _cameraTransform;

    private CharacterController _controller;
    private IInputProvider _inputProvider;
    private IGroundChecker _groundChecker;
    private IMovementController _movementController;
    private IFootstepController _footsteps;
    private OptimizedLookController _lookController;

    private MovementState _currentState;
    private float _lastJumpTime;

    private void Awake()
    {
        _inputProvider = new NewInputProvider();

        _controller = GetComponent<CharacterController>();
        _groundChecker = new OptimizedGroundChecker();
        _movementController = new KinematicMovementController(_config, transform);
        _footsteps = new OptimizedFootstepController(GetComponent<AudioSource>(), _config);

        if (_cameraTransform == null && Camera.main != null)
            _cameraTransform = Camera.main.transform;

        _lookController = new OptimizedLookController(transform, _cameraTransform, _config);

        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        bool isGamepad = _inputProvider.IsUsingGamepad;



        UpdateGroundState();
        UpdateMovementState();

        if (_config.CanJump)
            HandleJump();

        _movementController.UpdateMovement(_inputProvider.GetMovementInput(), _currentState, Time.deltaTime);
        _movementController.ApplyGravity(Time.deltaTime);
        _footsteps.UpdateFootsteps(_inputProvider.GetMovementInput(), _currentState, Time.deltaTime);
        //_lookController.UpdateLook(_inputProvider.GetLookInput(), Time.deltaTime);
        _lookController.UpdateLook(_inputProvider.GetLookInput(), Time.deltaTime, isGamepad);

        _controller.Move(_movementController.Velocity * Time.deltaTime);
    }

    private void OnDestroy()
    {
        (_inputProvider as IDisposable)?.Dispose();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void UpdateGroundState()
    {
        if (Time.time < _lastJumpTime + _config.JumpGroundingPreventionTime)
        {
            _currentState &= ~MovementState.Grounded;
            _currentState |= MovementState.InAir;
            return;
        }

        _groundChecker.CheckGround(
            transform.position + _controller.center,
            _controller.radius * _config.GroundCheckRadiusMultiplier,
            _controller.height,
            _config.GroundCheckDistance
        );

        if (_groundChecker.IsGrounded)
        {
            _currentState |= MovementState.Grounded;
            _currentState &= ~(MovementState.InAir | MovementState.Jumping);
        }
        else
        {
            _currentState &= ~MovementState.Grounded;
            _currentState |= MovementState.InAir;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void UpdateMovementState()
    {
        if (_inputProvider.GetSprintInput())
            _currentState |= MovementState.Sprinting;
        else
            _currentState &= ~MovementState.Sprinting;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void HandleJump()
    {
        if (_inputProvider.GetJumpInput() && (_currentState & MovementState.Grounded) != 0)
        {
            _movementController.Jump(_config.JumpForce);
            _currentState |= MovementState.Jumping;
            _currentState &= ~MovementState.Grounded;
            _lastJumpTime = Time.time;
        }
    }
}