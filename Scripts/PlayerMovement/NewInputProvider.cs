using System;
using UnityEngine;
using UnityEngine.InputSystem;

public sealed class NewInputProvider : IInputProvider, IDisposable
{
    private readonly @PlayerMovement _inputActions;
    private Vector2 _movement;
    private Vector2 _look;
    private bool _jumpTriggered;
    private bool _sprint;

    public bool IsUsingGamepad => 
    Gamepad.current != null && 
    (_inputActions.PlayerActions.Move.activeControl?.device is Gamepad ||
     _inputActions.PlayerActions.Look.activeControl?.device is Gamepad);

    public NewInputProvider()
    {
        _inputActions = new @PlayerMovement();
        _inputActions.PlayerActions.Enable();

        _inputActions.PlayerActions.Move.performed += ctx => _movement = ctx.ReadValue<Vector2>();
        _inputActions.PlayerActions.Move.canceled += _ => _movement = Vector2.zero;

        _inputActions.PlayerActions.Look.performed += ctx => _look = ctx.ReadValue<Vector2>();
        _inputActions.PlayerActions.Look.canceled += _ => _look = Vector2.zero;

        _inputActions.PlayerActions.Jump.started += _ => _jumpTriggered = true;
        _inputActions.PlayerActions.Sprint.started += _ => _sprint = true;
        _inputActions.PlayerActions.Sprint.canceled += _ => _sprint = false;
    }

    public Vector2 GetMovementInput() => _movement;
    public Vector2 GetLookInput() => _look;
    public bool GetJumpInput()
    {
        if (!_jumpTriggered) return false;
        _jumpTriggered = false;
        return true;
    }
    public bool GetSprintInput() => _sprint;
    public void Dispose() => _inputActions.Dispose();
}