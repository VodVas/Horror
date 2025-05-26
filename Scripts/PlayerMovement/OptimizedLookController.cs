using UnityEngine;
using System.Runtime.CompilerServices;

public sealed class OptimizedLookController
{
    private readonly Transform _playerTransform;
    private readonly Transform _cameraTransform;
    private readonly MovementConfig _config;
    private float _verticalRotation;

    public OptimizedLookController(Transform playerTransform, Transform cameraTransform, MovementConfig config)
    {
        _playerTransform = playerTransform;
        _cameraTransform = cameraTransform;
        _config = config;
    }

    //[MethodImpl(MethodImplOptions.AggressiveInlining)]
    //public void UpdateLook(Vector2 lookInput, float deltaTime)
    //{
    //    if (_cameraTransform == null) return;

    //    float sensitivity = _config.MouseSensitivity * deltaTime;

    //    _playerTransform.Rotate(0, lookInput.x * sensitivity, 0);

    //    _verticalRotation -= lookInput.y * sensitivity;
    //    _verticalRotation = Mathf.Clamp(_verticalRotation, -_config.MaxVerticalAngle, _config.MaxVerticalAngle);
    //    _cameraTransform.localEulerAngles = new Vector3(_verticalRotation, 0, 0);
    //}

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void UpdateLook(Vector2 lookInput, float deltaTime, bool isUsingGamepad)
    {
        if (_cameraTransform == null) return;

        float sensitivity = isUsingGamepad ?
            _config.GamepadLookSensitivity :
            _config.MouseSensitivity;

        _playerTransform.Rotate(0, lookInput.x * sensitivity * deltaTime, 0);

        _verticalRotation -= lookInput.y * sensitivity * deltaTime;
        _verticalRotation = Mathf.Clamp(_verticalRotation, -_config.MaxVerticalAngle, _config.MaxVerticalAngle);
        _cameraTransform.localEulerAngles = new Vector3(_verticalRotation, 0, 0);
    }
}