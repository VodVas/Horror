using System;
using System.Runtime.CompilerServices;

//using System.Runtime.CompilerServices;
//using UnityEngine;
//using UnityEngine.InputSystem;

//public sealed class NewInputProvider : MonoBehaviour, IInputProvider
//{
//    private readonly @PlayerMovement _inputActions;
//    private Vector2 _movement;
//    private Vector2 _look;
//    private bool _jumpTriggered;
//    private bool _sprint;
//    private bool _pushTriggered;

//    public bool IsUsingGamepad => 
//    Gamepad.current != null && 
//    (_inputActions.PlayerActions.Move.activeControl?.device is Gamepad ||
//     _inputActions.PlayerActions.Look.activeControl?.device is Gamepad);

//    public NewInputProvider()
//    {
//        _inputActions = new @PlayerMovement();
//        _inputActions.PlayerActions.Enable();

//        _inputActions.PlayerActions.Move.performed += ctx => _movement = ctx.ReadValue<Vector2>();
//        _inputActions.PlayerActions.Move.canceled += _ => _movement = Vector2.zero;

//        _inputActions.PlayerActions.Look.performed += ctx => _look = ctx.ReadValue<Vector2>();
//        _inputActions.PlayerActions.Look.canceled += _ => _look = Vector2.zero;

//        _inputActions.PlayerActions.Jump.started += _ => _jumpTriggered = true;
//        _inputActions.PlayerActions.Sprint.started += _ => _sprint = true;
//        _inputActions.PlayerActions.Sprint.canceled += _ => _sprint = false;
//        _inputActions.PlayerActions.PushAction.started += _ => _pushTriggered = true;
//    }

//    public bool GetPushInput()
//    {
//        if (!_pushTriggered) return false;

//        _pushTriggered = false;
//        return true;
//    }

//    public bool GetJumpInput()
//    {
//        if (!_jumpTriggered) return false;

//        _jumpTriggered = false;
//        return true;
//    }

//    public Vector2 GetMovementInput() => _movement;
//    public Vector2 GetLookInput() => _look;
//    public bool GetSprintInput() => _sprint;

//    private void OnDestroy()
//    {
//        _inputActions?.Dispose();
//    }
//}



using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
public sealed class NewInputProvider : MonoBehaviour, IInputProvider
{
    [SerializeField] private InputActionAsset _inputAsset;

    private PlayerMovement _inputActions;
    private Vector2 _movement;
    private Vector2 _look;
    private bool _jumpPressed;
    private bool _pushPressed;
    private bool _sprintPressed;

    public Vector2 GetMovementInput() => _movement;
    public Vector2 GetLookInput() => _look;
    public bool GetJumpInput() => ReadButton(ref _jumpPressed);
    public bool GetPushInput() => ReadButton(ref _pushPressed);
    public bool GetSprintInput() => _sprintPressed;
    public bool IsUsingGamepad { get; private set; }

    private void Awake()
    {
        InitializeInputSystem();
        SetupCallbacks();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void InitializeInputSystem()
    {
        _inputActions =  new PlayerMovement();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void SetupCallbacks()
    {
        _inputActions.PlayerActions.Move.performed += ctx => _movement = ctx.ReadValue<Vector2>();
        _inputActions.PlayerActions.Move.canceled += _ => _movement = Vector2.zero;

        _inputActions.PlayerActions.Look.performed += ctx => _look = ctx.ReadValue<Vector2>();
        _inputActions.PlayerActions.Look.canceled += _ => _look = Vector2.zero;

        _inputActions.PlayerActions.Jump.started += _ => _jumpPressed = true;
        _inputActions.PlayerActions.PushAction.started += _ => _pushPressed = true;
        _inputActions.PlayerActions.Sprint.started += _ => _sprintPressed = true;
        _inputActions.PlayerActions.Sprint.canceled += _ => _sprintPressed = false;

        _inputActions.Enable();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool ReadButton(ref bool buttonState)
    {
        if (!buttonState) return false;
        buttonState = false;
        return true;
    }

    private void Update() => UpdateDeviceState();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void UpdateDeviceState()
    {
        IsUsingGamepad = Gamepad.current != null &&
            (_inputActions.PlayerActions.Move.activeControl?.device is Gamepad ||
             _inputActions.PlayerActions.Look.activeControl?.device is Gamepad);
    }

    private void OnDestroy()
    {
        if (_inputActions != null)
        {
            _inputActions.Disable();
            _inputActions.Dispose();
        }
    }
}