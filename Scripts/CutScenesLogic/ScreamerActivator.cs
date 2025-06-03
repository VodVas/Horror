using UnityEngine;
using System.Collections;
using System;

public class ScreamerActivator : MonoBehaviour
{
    private enum ActivationState
    {
        Idle,
        ClosingDoor,
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

    [Header("Door Settings")]
    [SerializeField] private Transform _doorTransform;
    [SerializeField] private Vector3 _doorCloseAxis = Vector3.up;
    [SerializeField] private float _doorCloseAngle = 90f;
    [SerializeField] private float _doorCloseSpeed = 2f;
    [SerializeField] private AudioSource _doorAudioSource;
    [SerializeField] private AudioClip _doorCloseSound;

    [Header("References")]
    [SerializeField] private EmoSetter _emoSetter;
    [SerializeField] private SecondCutsceneActivator _secondCutscene;
    [SerializeField] private AudioSource _screamAudioSource;
    [SerializeField] private AudioClip _screamSound;
    [SerializeField] private ObjectOnceShaker _cameraShaker;

    private ActivationState _currentState = ActivationState.Idle;
    private Quaternion _initialDoorRotation;
    private Quaternion _targetDoorRotation;
    private Coroutine _activationCoroutine;
    private float _doorCloseProgress;

    public event Action OnSequenceCompleted;

    private void Awake()
    {
        ValidateReferences();
        InitializeDoorRotations();
    }

    private void OnEnable()
    {
        if (_secondCutscene != null)
            _secondCutscene.EndOf2Scene += OnSceneEnded;
    }

    private void OnDisable()
    {
        if (_secondCutscene != null)
            _secondCutscene.EndOf2Scene -= OnSceneEnded;

        StopActivationSequence();
    }

    private void Update()
    {
        switch (_currentState)
        {
            case ActivationState.ClosingDoor:
                UpdateDoorClosing();
                break;
            case ActivationState.Moving:
                UpdateMovement();
                break;
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
        // Step 1: Start closing door
        _currentState = ActivationState.ClosingDoor;
        PlayDoorCloseSound();

        // Step 2: Set emotion while door is closing
        SetEmotion();

        // Wait for door to close
        yield return new WaitWhile(() => _currentState == ActivationState.ClosingDoor);

        // Step 3: Wait random delay
        _currentState = ActivationState.WaitingDelay;
        float delay = UnityEngine.Random.Range(_minDelay, _maxDelay);
        yield return new WaitForSeconds(delay);

        // Step 4: Start moving
        PlayScreamSound();
        _currentState = ActivationState.Moving;

        // Wait for movement to complete
        yield return new WaitWhile(() => _currentState == ActivationState.Moving);

        // Sequence completed
        _currentState = ActivationState.Completed;
        OnSequenceCompleted?.Invoke();
    }

    private void UpdateDoorClosing()
    {
        if (_doorTransform == null)
        {
            _currentState = ActivationState.WaitingDelay;
            return;
        }

        float rotationStep = _doorCloseSpeed * Time.deltaTime * 100f;
        _doorTransform.rotation = Quaternion.RotateTowards(
            _doorTransform.rotation,
            _targetDoorRotation,
            rotationStep
        );

        _doorCloseProgress = 1f - (Quaternion.Angle(_doorTransform.rotation, _targetDoorRotation) / _doorCloseAngle);

        if (Quaternion.Angle(_doorTransform.rotation, _targetDoorRotation) < 0.5f)
        {
            _doorTransform.rotation = _targetDoorRotation;
            _currentState = ActivationState.WaitingDelay;
        }
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

        _cameraShaker.Shake();
    }

    private void PlayDoorCloseSound()
    {
        if (_doorAudioSource != null && _doorCloseSound != null)
        {
            _doorAudioSource.PlayOneShot(_doorCloseSound);
        }
    }

    private void PlayScreamSound()
    {
        if (_screamAudioSource != null && _screamSound != null)
        {
            _screamAudioSource.PlayOneShot(_screamSound);
        }
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

        if (_doorCloseSpeed <= 0f)
        {
            Debug.LogError("[ScreamerActivator] Door close speed must be greater than 0", this);
            _doorCloseSpeed = 2f;
        }

        if (_minDelay > _maxDelay)
        {
            Debug.LogError("[ScreamerActivator] Min delay cannot be greater than max delay", this);
            float temp = _minDelay;
            _minDelay = _maxDelay;
            _maxDelay = temp;
        }
    }

    private void InitializeDoorRotations()
    {
        if (_doorTransform != null)
        {
            _initialDoorRotation = _doorTransform.rotation;
            _targetDoorRotation = _initialDoorRotation * Quaternion.AngleAxis(_doorCloseAngle, _doorCloseAxis);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (_targetPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_targetPoint.position, _stoppingDistance);

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, _targetPoint.position);
        }

        if (_doorTransform != null && Application.isPlaying)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(_doorTransform.position, _doorTransform.forward * 2f);
        }
    }
#endif
}
