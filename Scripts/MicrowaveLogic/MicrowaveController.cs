using System;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using DG.Tweening;

[RequireComponent(typeof(AudioSource))]
public sealed class MicrowaveController : MonoBehaviour
{
    public event Action<MicrowaveState> OnStateChanged;

    [Header("Components")]
    [SerializeField] private MicrowaveDoor _door;
    [SerializeField] private MicrowaveButton _button;
    [SerializeField] private AudioSource _audioSource;

    [Header("Visuals")]
    [SerializeField] private Transform _plate;
    [SerializeField] private Transform _light;
    [SerializeField] private float _rotationSpeed = 90f;

    [Header("Break")]
    [SerializeField] private GameObject _lightingEffect;
    [SerializeField] private float _blinkDuration = 0.1f;
    [SerializeField] private AudioClip _breakSound;

    [Header("Colliders")]
    [SerializeField] private Collider _buttonCollider;
    [SerializeField] private BoxCollider _cookingZone;

    [Header("Settings")]
    [SerializeField] private float _cookingDuration = 5f;
    [SerializeField] private AudioClip _cookingSound;
    [SerializeField] private AudioClip _readySound;

    private MicrowaveState _currentState;
    private CancellationTokenSource _cookingCts;
    private CancellationTokenSource _lightingCts;
    private Tweener _rotationTween;

    public MicrowaveState CurrentState => _currentState;
    public bool IsDoorOpen => _currentState == MicrowaveState.DoorOpen;
    public bool IsDoorClosed => _currentState == MicrowaveState.DoorClosed;
    public bool IsCooking => _currentState == MicrowaveState.Cooking;

    private void Awake()
    {
        ValidateComponents();
        _audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        SetState(MicrowaveState.DoorClosed);
        InitializeComponents();
    }

    private void OnDestroy()
    {
        _cookingCts?.Cancel();
        _cookingCts?.Dispose();
        _lightingCts?.Cancel();
        _lightingCts?.Dispose();
    }

    private void ValidateComponents()
    {
        if (!_door) Debug.LogError("MicrowaveDoor not assigned", this);
        if (!_button) Debug.LogError("MicrowaveButton not assigned", this);

        _audioSource = GetComponent<AudioSource>();
    }

    private void InitializeComponents()
    {
        if (_door) _door.Initialize(this);
        if (_button) _button.Initialize(this);
    }

    public void RequestToggleDoor()
    {
        Debug.Log("RequestToggleDoor()");
        if (IsCooking) return;

        switch (_currentState)
        {
            case MicrowaveState.DoorClosed:
                Debug.Log("MicrowaveState.DoorClosed:");
                //SetState(MicrowaveState.DoorClosed);
                SetState(MicrowaveState.DoorOpen);
                break;

            case MicrowaveState.Ready:
                Debug.Log("MicrowaveState.Ready:");
                SetState(MicrowaveState.DoorOpen);

                break;

            case MicrowaveState.DoorOpen:
                Debug.Log("MicrowaveState.DoorOpen:");
                SetState(MicrowaveState.DoorClosed);
                break;
        }
    }

    public void RequestStartCooking()
    {
        if (_currentState == MicrowaveState.DoorClosed)
        {
            StartCooking();
        }
    }

    private void StartCooking()
    {
        if (HasDrinksInside())
        {
            Debug.Log("Microwave blocked: Cannot cook drinks! Microwave is broken now!", this);
            BreakMicrowave();
            return;
        }

        SetState(MicrowaveState.Cooking);

        _cookingCts?.Cancel();
        _cookingCts = new CancellationTokenSource();

        CookingProcess(_cookingCts.Token).Forget();
    }

    private bool HasDrinksInside()
    {
        if (_cookingZone == null)
        {
            Debug.LogError("Cooking zone collider not assigned!", this);
            return false;
        }

        Vector3 center = _cookingZone.transform.TransformPoint(_cookingZone.center);
        Vector3 halfExtents = _cookingZone.size * 0.5f;
        Quaternion rotation = _cookingZone.transform.rotation;

        Collider[] colliders = Physics.OverlapBox(center, halfExtents, rotation);

        foreach (var collider in colliders)
        {
            if (collider.TryGetComponent(out Drink _))
                return true;
        }
        return false;
    }

    private void BreakMicrowave()
    {
        _lightingCts?.Cancel();
        _lightingCts?.Dispose();

        _lightingCts = new CancellationTokenSource();

        LightingBlinkEffect(_lightingCts.Token).Forget();
        PlayBreakSound();

        if (_buttonCollider != null)
            _buttonCollider.enabled = false;

        Debug.Log("Microwave is broken! Button disabled.");
    }

    private void PlayBreakSound()
    {
        if (_breakSound != null && _audioSource != null)
            _audioSource.PlayOneShot(_breakSound);
    }

    private async UniTaskVoid LightingBlinkEffect(CancellationToken token)
    {
        if (_lightingEffect == null) return;

        try
        {
            _lightingEffect.SetActive(true);
            await UniTask.Delay(TimeSpan.FromSeconds(_blinkDuration), cancellationToken: token);
            _lightingEffect.SetActive(false);
        }
        catch (OperationCanceledException)
        {
            _lightingEffect.SetActive(false);
        }
    }

    private async UniTaskVoid CookingProcess(CancellationToken token)
    {
        try
        {
            await UniTask.Delay(TimeSpan.FromSeconds(_cookingDuration), cancellationToken: token);

            if (!token.IsCancellationRequested)
            {
                SetState(MicrowaveState.Ready);
            }
        }
        catch (OperationCanceledException)
        {
        }
    }

    private void SetState(MicrowaveState newState)
    {
        if (_currentState == newState) return;

        _currentState = newState;
        HandleStateTransition(newState);
        OnStateChanged?.Invoke(newState);
    }

    private void HandleStateTransition(MicrowaveState newState)
    {
        switch (newState)
        {
            case MicrowaveState.DoorClosed:
                break;

            case MicrowaveState.Cooking:
                StartPlateAnimation();
                SetLight(true);
                PlaySound(_cookingSound);

                break;

            case MicrowaveState.Ready:
                StopSound();
                StopPlateAnimation();
                SetLight(false);
                if (_readySound) PlaySound(_readySound);

                break;

            case MicrowaveState.DoorOpen:
                if (_currentState == MicrowaveState.Cooking)
                {
                    StopPlateAnimation();
                    SetLight(false);
                }
                StopSound();

                break;
        }
    }

    private void SetLight(bool enabled)
    {
        if (_light)
        {
            _light.gameObject.SetActive(enabled);
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (!_audioSource || !clip) return;

        _audioSource.clip = clip;
        _audioSource.Play();
    }

    private void StopSound()
    {
        if (_audioSource) _audioSource.Stop();
    }

    private void StartPlateAnimation()
    {
        if (!_plate) return;

        _rotationTween = _plate.DOLocalRotate(
            new Vector3(0, 360, 0),
            360f / _rotationSpeed,
            RotateMode.FastBeyond360
        )
        .SetLoops(-1)
        .SetEase(Ease.Linear);
    }

    private void StopPlateAnimation()
    {
        _rotationTween?.Kill(); 

        if (_plate) _plate.localRotation = Quaternion.identity;
    }
}