//using UnityEngine;
//using DG.Tweening;

//public class DoorController : MonoBehaviour, IInteractable
//{
//    [SerializeField] private MicrowaveController _microwaveController;
//    [SerializeField] private float _animationDuration = 1f;
//    [SerializeField] private Ease movementEase = Ease.Linear;

//    private Quaternion _closedRotation;
//    private Quaternion _openRotation;
//    private Tweener _doorTween;

//    private void Awake()
//    {
//        _closedRotation = transform.rotation;
//        _openRotation = _closedRotation * Quaternion.Euler(0, 90, 0);
//    }

//    private void OnDestroy() => _doorTween?.Kill();

//    public void Interact() => ToggleDoor();

//    private void ToggleDoor()
//    {
//        if (_microwaveController.IsCooking) return;
//        AnimateDoor(transform.rotation == _closedRotation ? _openRotation : _closedRotation);
//    }

//    public void OpenDoor() => AnimateDoor(_openRotation);

//    private void AnimateDoor(Quaternion targetRotation)
//    {
//        _doorTween?.Kill();
//        _doorTween = transform.DORotateQuaternion(targetRotation, _animationDuration)
//            .SetEase(movementEase)
//            .OnComplete(() => _microwaveController.NotifyDoorState(targetRotation == _closedRotation))
//            .OnKill(() => _doorTween = null);
//    }
//}

using UnityEngine;
using DG.Tweening;

public class MicrowaveDoorController : MonoBehaviour, IInteractable
{
    [SerializeField] private MicrowaveController _microwaveController;
    [SerializeField] private float _animationDuration = 1f;
    [SerializeField] private Ease movementEase = Ease.Linear;

    private Quaternion _closedRotation;
    private Quaternion _openRotation;
    private Tweener _doorTween;

    private void Awake()
    {
        _closedRotation = transform.rotation;
        _openRotation = _closedRotation * Quaternion.Euler(0, 90, 0);
    }

    private void OnDestroy() => _doorTween?.Kill();

    public void Interact() => ToggleDoor();

    public void OpenDoor() => AnimateDoor(_openRotation);

    private void ToggleDoor()
    {
        if (_microwaveController.IsCooking) return;
        AnimateDoor(transform.rotation == _closedRotation ? _openRotation : _closedRotation);
    }

    private void AnimateDoor(Quaternion targetRotation)
    {
        _doorTween?.Kill();
        _doorTween = transform.DORotateQuaternion(targetRotation, _animationDuration)
            .SetEase(movementEase)
            .OnComplete(() => _microwaveController.NotifyDoorState(targetRotation == _closedRotation))
            .OnKill(() => _doorTween = null);
    }
}
