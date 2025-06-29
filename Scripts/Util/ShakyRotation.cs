using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Transform))]
public class ShakyRotation : MonoBehaviour
{
    [SerializeField] private float _duration = 2f;
    [SerializeField] private float _shakeStrength = 10f;
    [SerializeField] private int _shakeVibrato = 5;
    [SerializeField] private float _shakeRandomness = 1f;
    [SerializeField] private float _targetAngleY = 175f;

    private Vector3 _targetRotation;
    private Sequence _rotationSequence;

    private void OnDestroy() => _rotationSequence?.Kill();

    [ContextMenu("Execute")]
    public void Execute()
    {
        RotateWithShake(_targetAngleY);
    }

    private void RotateWithShake(float targetAngleY)
    {
        _rotationSequence?.Kill();

        _targetRotation = new Vector3(0, targetAngleY, 0);
        _rotationSequence = DOTween.Sequence();

        _rotationSequence.Append(
            transform.DORotate(_targetRotation, _duration, RotateMode.Fast)
            .SetEase(Ease.InOutSine)
        );

        _rotationSequence.Join(
            transform.DOShakeRotation(_duration, _shakeStrength, _shakeVibrato, _shakeRandomness, false, ShakeRandomnessMode.Harmonic)
        );

        _rotationSequence.OnComplete(() => transform.eulerAngles = _targetRotation);
    }
}