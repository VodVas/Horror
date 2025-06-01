using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;

public class CoffeeMachinesEnabler : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform _handle;
    [SerializeField] private float _rotationAngle = 30f;
    [SerializeField] private float _animationDuration = 0.5f;
    [SerializeField] private Ease _easeType = Ease.OutBack;
    [SerializeField] private GameObject _coffeeStream;
    [SerializeField] private float _cookingDuration = 3f;

    private CancellationTokenSource _cookingCts;
    private bool _isCooking;
    private Tweener _currentTween;
    private Vector3 _initialHandleRotation;

    public bool CanInteract => !_isCooking;
    public Transform Transform => transform;

    private void Awake()
    {
        if (_handle == null)
        {
            Debug.LogError("[CoffeeMachine] Handle not assigned", this);
            enabled = false;
            return;
        }

        if (_coffeeStream == null)
        {
            Debug.LogError("[CoffeeMachine] Coffee stream not assigned", this);
            enabled = false;
            return;
        }

        _initialHandleRotation = _handle.localEulerAngles;
        _coffeeStream.SetActive(false);
    }

    private void OnDestroy()
    {
        _currentTween?.Kill();
        _cookingCts?.Cancel();
        _cookingCts?.Dispose();
    }

    public void Interact()
    {
        if (_isCooking) return;

        StartCoffeeMaking().Forget();
    }

    private async UniTaskVoid StartCoffeeMaking()
    {
        _isCooking = true;
        _cookingCts?.Dispose();
        _cookingCts = new CancellationTokenSource();

        try
        {
            await AnimateHandle(true);

            _coffeeStream.SetActive(true);
            await UniTask.Delay(TimeSpan.FromSeconds(_cookingDuration),
                cancellationToken: _cookingCts.Token);

            _coffeeStream.SetActive(false);
            await AnimateHandle(false);
        }
        catch (OperationCanceledException)
        {
            _coffeeStream.SetActive(false);
            await AnimateHandle(false, true);
        }
        finally
        {
            _isCooking = false;
        }
    }

    private UniTask AnimateHandle(bool isDown, bool immediate = false)
    {
        _currentTween?.Kill();

        var targetRotation = isDown
            ? new Vector3(0f, -_rotationAngle, 0f)
            : _initialHandleRotation;

        if (immediate)
        {
            _handle.localEulerAngles = targetRotation;
            return UniTask.CompletedTask;
        }

        var tcs = new UniTaskCompletionSource();

        _currentTween = _handle
            .DOLocalRotate(targetRotation, _animationDuration)
            .SetEase(_easeType)
            .OnKill(() => tcs.TrySetCanceled())
            .OnComplete(() => {
                _currentTween = null;
                tcs.TrySetResult();
            });

        return tcs.Task;
    }
}