using UnityEngine;
using System.Runtime.CompilerServices;

[RequireComponent(typeof(CharacterController))]
public sealed class OptimizedFpsMovement : MonoBehaviour
{
    [SerializeField] private MovementConfig _config;
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private LayerMask _groundMask = -1;
    [SerializeField] private GroundCheckMethod _groundCheckMethod = GroundCheckMethod.Combined;
    [SerializeField] private GroundCheckDebugSettings _debugSettings = new GroundCheckDebugSettings();
    [SerializeField] private NewInputProvider _input;

    private CharacterController _controller;
    private OptimizedLookController _lookController;
    private EnhancedGroundChecker _groundChecker;
    private IMovementController _movementController;
    private IFootstepController _footsteps;
    private MovementState _currentState;
    private float _lastJumpTime;
    private float _lastGroundedTime;
    private bool _wasGroundedLastFrame;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();

        _groundChecker = new EnhancedGroundChecker
        {
            CheckMethod = _groundCheckMethod,
            DebugSettings = {
                showDebugInfo = _debugSettings.showDebugInfo,
                showGizmos = _debugSettings.showGizmos,
                groundedColor = _debugSettings.groundedColor,
                airborneColor = _debugSettings.airborneColor,
                rayColor = _debugSettings.rayColor,
                hitPointColor = _debugSettings.hitPointColor
            }
        };

        _movementController = new KinematicMovementController(_config, transform);
        _footsteps = new OptimizedFootstepController(GetComponent<AudioSource>(), _config);

        if (_cameraTransform == null && Camera.main != null)
            _cameraTransform = Camera.main.transform;

        _lookController = new OptimizedLookController(transform, _cameraTransform, _config);

        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        bool isGamepad = _input.IsUsingGamepad;

        UpdateGroundState();
        UpdateMovementState();

        if (_config.CanJump)
            HandleJump();

        _movementController.UpdateMovement(_input.GetMovementInput(), _currentState, Time.deltaTime);
        _movementController.ApplyGravity(Time.deltaTime);
        _footsteps.UpdateFootsteps(_input.GetMovementInput(), _currentState, Time.deltaTime);
        _lookController.UpdateLook(_input.GetLookInput(), Time.deltaTime, isGamepad);

        _controller.Move(_movementController.Velocity * Time.deltaTime);

        _groundChecker.DrawDebugVisualization(
            transform.position + _controller.center,
            _controller.radius,
            _controller.height,
            _config.GroundCheckDistance
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void UpdateGroundState()
    {
        bool preventGroundCheck = Time.time < _lastJumpTime + _config.JumpGroundingPreventionTime;
        bool isMovingUp = _movementController.Velocity.y > 0.1f;

        if (preventGroundCheck || isMovingUp)
        {
            _currentState &= ~MovementState.Grounded;
            _currentState |= MovementState.InAir;
            _wasGroundedLastFrame = false;
            return;
        }

        _groundChecker.CheckGround(
            transform.position + _controller.center,
            _controller.radius * _config.GroundCheckRadiusMultiplier,
            _controller.height,
            _config.GroundCheckDistance,
            _groundMask
        );

        bool isGrounded = _groundChecker.IsGrounded && _groundChecker.GroundDistance <= _config.GroundCheckDistance;

        if (isGrounded)
        {
            if (!_wasGroundedLastFrame)
            {
                _lastGroundedTime = Time.time;
            }

            _currentState |= MovementState.Grounded;
            _currentState &= ~(MovementState.InAir | MovementState.Jumping);
            _wasGroundedLastFrame = true;
        }
        else
        {
            _currentState &= ~MovementState.Grounded;
            _currentState |= MovementState.InAir;
            _wasGroundedLastFrame = false;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void UpdateMovementState()
    {
        if (_input.GetSprintInput())
            _currentState |= MovementState.Sprinting;
        else
            _currentState &= ~MovementState.Sprinting;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void HandleJump()
    {
        bool canJump = (_currentState & MovementState.Grounded) != 0 &&
                      !(_currentState & MovementState.Jumping).HasFlag(MovementState.Jumping) &&
                      Time.time > _lastJumpTime + 0.2f;

        if (_input.GetJumpInput() && canJump)
        {
            _movementController.Jump(_config.JumpForce);
            _currentState |= MovementState.Jumping;
            _currentState &= ~MovementState.Grounded;
            _lastJumpTime = Time.time;
            _wasGroundedLastFrame = false;
        }
    }

    private void OnValidate()
    {
        if (_groundChecker != null)
        {
            _groundChecker.CheckMethod = _groundCheckMethod;
            _groundChecker.DebugSettings.showDebugInfo = _debugSettings.showDebugInfo;
            _groundChecker.DebugSettings.showGizmos = _debugSettings.showGizmos;
            _groundChecker.DebugSettings.groundedColor = _debugSettings.groundedColor;
            _groundChecker.DebugSettings.airborneColor = _debugSettings.airborneColor;
            _groundChecker.DebugSettings.rayColor = _debugSettings.rayColor;
            _groundChecker.DebugSettings.hitPointColor = _debugSettings.hitPointColor;
        }
    }
}