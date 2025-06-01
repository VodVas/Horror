using UnityEngine;
using DG.Tweening;

public sealed class MicrowaveDoor : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform _doorTransform;
    [SerializeField] private float _animationDuration = 0.5f;
    [SerializeField] private float _openAngle = 90f;
    [SerializeField] private Ease _animationEase = Ease.OutBack;

    private MicrowaveController _controller;
    private Quaternion _closedRotation;
    private Quaternion _openRotation;
    private Tweener _animationTween;
    private bool _isInitialized;

    public bool CanInteract => _isInitialized && !_controller.IsCooking;

    public Transform Transform => GetComponent<Transform>();

    private void Awake()
    {
        if (!_doorTransform) _doorTransform = transform;
        _closedRotation = _doorTransform.rotation;
        _openRotation = _closedRotation * Quaternion.Euler(0, _openAngle, 0);
    }

    private void OnDestroy()
    {
        _animationTween?.Kill();
        if (_controller) _controller.OnStateChanged -= HandleStateChanged;
    }

    public void Initialize(MicrowaveController controller)
    {
        _controller = controller;
        _controller.OnStateChanged += HandleStateChanged;
        _isInitialized = true;

        UpdateDoorPosition(_controller.CurrentState);
    }

    public void Interact()
    {
        if (!CanInteract) return;
        _controller.RequestToggleDoor();
    }

    private void HandleStateChanged(MicrowaveState newState)
    {
        UpdateDoorPosition(newState);
    }

    private void UpdateDoorPosition(MicrowaveState state)
    {
        _animationTween?.Kill();

        Quaternion targetRotation = (state == MicrowaveState.DoorOpen) ? _openRotation : _closedRotation;

        _animationTween = _doorTransform
            .DORotateQuaternion(targetRotation, _animationDuration)
            .SetEase(_animationEase)
            .OnKill(() => _animationTween = null);
    }
}