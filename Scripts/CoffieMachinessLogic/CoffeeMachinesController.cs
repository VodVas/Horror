using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;

public class CoffeeMachinesController : MonoBehaviour, IInteractable
{
    [Header("Animation Settings")]
    [SerializeField] private Transform _handle;
    [SerializeField] private float _rotationAngle = 30f;
    [SerializeField] private float _animationDuration = 0.5f;
    [SerializeField] private Ease _easeType = Ease.OutBack;

    [Header("Coffee Settings")]
    [SerializeField] private GameObject _coffeeStream;
    [SerializeField] private GameObject _coffeeStream2;
    [SerializeField] private float _cookingDuration = 3f;

    [Header("Sound Settings")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _handleSound;
    [SerializeField] private AudioClip _pouringSound;

    private CancellationTokenSource _cookingCts;
    private bool _isCooking;
    private bool _hasCupInserted;
    private bool _isCupFilled;
    private Tweener _currentTween;
    private Vector3 _initialHandleRotation;

    public bool CanInteract => _hasCupInserted && !_isCooking && !_isCupFilled;
    public Transform Transform => transform;

    public event Action OnCoffeeComplete;

    private void Awake()
    {
        if (_handle == null)
        {
            Debug.LogError("[CoffeeMachine] Handle not assigned", this);
            enabled = false;
            return;
        }

        if (_coffeeStream == null || _coffeeStream2 == null)
        {
            Debug.LogError("[CoffeeMachine] Coffee streams not assigned", this);
            enabled = false;
            return;
        }

        _initialHandleRotation = _handle.localEulerAngles;
        _coffeeStream.SetActive(false);
        _coffeeStream2.SetActive(false);
    }

    private void OnDestroy()
    {
        _currentTween?.Kill();
        _cookingCts?.Cancel();
        _cookingCts?.Dispose();
        StopAllSounds();
    }

    public void Interact()
    {
        if (!CanInteract) return;
        StartCoffeeMaking().Forget();
    }

    public void SetCupState(bool inserted, bool filled = false)
    {
        _hasCupInserted = inserted;
        _isCupFilled = filled;

        if (!inserted)
        {
            _isCupFilled = false;
        }
    }

    private async UniTaskVoid StartCoffeeMaking()
    {
        _isCooking = true;
        _cookingCts?.Dispose();
        _cookingCts = new CancellationTokenSource();

        try
        {
            await AnimateHandle(true);
            PlayPouringSound();
            _coffeeStream.SetActive(true);
            _coffeeStream2.SetActive(true);

            await UniTask.Delay(TimeSpan.FromSeconds(_cookingDuration),
                cancellationToken: _cookingCts.Token);

            _coffeeStream.SetActive(false);
            _coffeeStream2.SetActive(false);
            StopPouringSound();
            await AnimateHandle(false);

            _isCupFilled = true;
            OnCoffeeComplete?.Invoke();
        }
        catch (OperationCanceledException)
        {
            _coffeeStream.SetActive(false);
            _coffeeStream2.SetActive(false);
            StopPouringSound();
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

        PlayHandleSound();

        var tcs = new UniTaskCompletionSource();
        _currentTween = _handle
            .DOLocalRotate(targetRotation, _animationDuration)
            .SetEase(_easeType)
            .OnKill(() => tcs.TrySetCanceled())
            .OnComplete(() => tcs.TrySetResult());

        return tcs.Task;
    }

    private void PlayHandleSound()
    {
        if (_audioSource == null || _handleSound == null) return;
        _audioSource.PlayOneShot(_handleSound);
    }

    private void PlayPouringSound()
    {
        if (_audioSource == null || _pouringSound == null) return;
        _audioSource.clip = _pouringSound;
        _audioSource.loop = true;
        _audioSource.Play();
    }

    private void StopPouringSound()
    {
        if (_audioSource == null || !_audioSource.isPlaying) return;
        _audioSource.Stop();
    }

    private void StopAllSounds()
    {
        if (_audioSource == null) return;
        _audioSource.Stop();
    }
}