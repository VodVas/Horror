using System.Runtime.CompilerServices;
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
    private bool _menuPressed;

    public Vector2 GetMovementInput() => _movement;
    public Vector2 GetLookInput() => _look;
    public bool GetJumpInput() => ReadButton(ref _jumpPressed);
    public bool GetPushInput() => ReadButton(ref _pushPressed);
    public bool GetSprintInput() => _sprintPressed;
    public bool GetMenuInput() => ReadButton(ref _menuPressed);
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
        _inputActions.PlayerActions.Menu.started += _ => _menuPressed = true;

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