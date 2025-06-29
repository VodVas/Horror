using UnityEngine;
using System.Collections;
using System;

public class ScreamerActivator : MonoBehaviour
{
    private enum ActivationState
    {
        Idle,
        WaitingDelay,
        Moving,
        Completed
    }

    [Header("Movement Settings")]
    [SerializeField] private Transform _targetPoint;
    [SerializeField] private float _speed = 1f;
    [SerializeField] private float _stoppingDistance = 0.1f;

    [Header("Activation Settings")]
    [SerializeField] private float _minDelay = 3f;
    [SerializeField] private float _maxDelay = 6f;

    [Header("References")]
    [SerializeField] private EmoSetter _emoSetter;
    [SerializeField] private AudioSource _screamAudioSource;
    [SerializeField] private AudioClip _screamSound;
    [SerializeField] private ObjectOnceShaker _cameraShaker;
    [SerializeField] private SkinnedMeshRenderer _bodyMeshRenderer;
    [SerializeField] private SkinnedMeshRenderer _headMeshRenderer;

    [SerializeField] private GiggleSceneLogic _giggleSceneLogic;

    private ActivationState _currentState = ActivationState.Idle;
    private Coroutine _activationCoroutine;

    public event Action OnSequenceCompleted;

    private void Awake()
    {
        ValidateReferences();
    }

    private void OnEnable()
    {
        if (_giggleSceneLogic != null)
            _giggleSceneLogic.EndOfScene += OnSceneEnded;
    }

    private void OnDisable()
    {
        if (_giggleSceneLogic != null)
            _giggleSceneLogic.EndOfScene -= OnSceneEnded;

        StopActivationSequence();
    }

    private void Update()
    {
        if (_currentState == ActivationState.Moving)
        {
            UpdateMovement();
        }
    }

    private void OnSceneEnded()
    {
        if (_currentState != ActivationState.Idle)
        {
            Debug.LogWarning("[ScreamerActivator] Sequence already in progress, ignoring new trigger");
            return;
        }

        StartActivationSequence();
    }

    private void StartActivationSequence()
    {
        if (_activationCoroutine != null)
            StopCoroutine(_activationCoroutine);

        _activationCoroutine = StartCoroutine(ActivationSequence());
    }

    private void StopActivationSequence()
    {
        if (_activationCoroutine != null)
        {
            StopCoroutine(_activationCoroutine);
            _activationCoroutine = null;
        }

        _currentState = ActivationState.Idle;
    }

    private IEnumerator ActivationSequence()
    {
        ActivateMonster();
        SetEmotion();

        _currentState = ActivationState.WaitingDelay;
        float delay = UnityEngine.Random.Range(_minDelay, _maxDelay);
        yield return new WaitForSeconds(delay);

        PlayScreamSound();
        _currentState = ActivationState.Moving;

        yield return new WaitWhile(() => _currentState == ActivationState.Moving);

        _currentState = ActivationState.Completed;
        OnSequenceCompleted?.Invoke();
    }

    private void UpdateMovement()
    {
        if (_targetPoint == null)
        {
            _currentState = ActivationState.Completed;
            return;
        }

        Vector3 targetPosition = _targetPoint.position;
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            _speed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, targetPosition) <= _stoppingDistance)
        {
            _currentState = ActivationState.Completed;
        }

        if (_cameraShaker != null)
            _cameraShaker.Shake();
    }

    private void PlayScreamSound()
    {
        if (_screamAudioSource != null && _screamSound != null)
        {
            _screamAudioSource.PlayOneShot(_screamSound);
        }
    }

    private void ActivateMonster()
    {
        if (_bodyMeshRenderer != null)
            _bodyMeshRenderer.enabled = true;

        if (_headMeshRenderer != null)
            _headMeshRenderer.enabled = true;
    }

    private void SetEmotion()
    {
        if (_emoSetter != null)
        {
            _emoSetter.SetEnterEmotion();
        }
    }

    private void ValidateReferences()
    {
        if (_speed <= 0f)
        {
            Debug.LogError("[ScreamerActivator] Speed must be greater than 0", this);
            _speed = 1f;
        }

        if (_minDelay > _maxDelay)
        {
            Debug.LogError("[ScreamerActivator] Min delay cannot be greater than max delay", this);
            float temp = _minDelay;
            _minDelay = _maxDelay;
            _maxDelay = temp;
        }
    }
}