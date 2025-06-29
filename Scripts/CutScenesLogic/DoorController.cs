using UnityEngine;
using System;
using System.Collections;

public class DoorController : MonoBehaviour
{
    [Header("Door Settings")]
    [SerializeField] private Transform _doorTransform;
    [SerializeField] private Vector3 _rotationAxis = Vector3.up;
    [SerializeField] private float _targetAngle = 90f;
    [SerializeField] private float _rotationSpeed = 2f;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _closeSound;

    private Quaternion _initialRotation;
    private Quaternion _targetRotation;
    private Coroutine _rotationCoroutine;
    private bool _isClosing;
    private float _closeProgress;

    public event Action OnCloseStarted;
    public event Action OnCloseCompleted;
    public event Action<float> OnCloseProgress;

    public bool IsClosing => _isClosing;
    public float CloseProgress => _closeProgress;

    private void Awake()
    {
        ValidateReferences();
        InitializeRotations();
    }

    private void Start()
    {
        CloseDoor();
    }

    private void OnDisable()
    {
        StopClosing();
    }

    public void CloseDoor()
    {
        if (_isClosing)
        {
            Debug.LogWarning("[DoorController] Door is already closing");
            return;
        }

        if (_rotationCoroutine != null)
            StopCoroutine(_rotationCoroutine);

        _rotationCoroutine = StartCoroutine(CloseRoutine());
    }

    public void StopClosing()
    {
        if (_rotationCoroutine != null)
        {
            StopCoroutine(_rotationCoroutine);
            _rotationCoroutine = null;
        }

        _isClosing = false;
    }

    private IEnumerator CloseRoutine()
    {
        _isClosing = true;
        _closeProgress = 0f;

        OnCloseStarted?.Invoke();
        PlayCloseSound();

        float totalAngle = Quaternion.Angle(_doorTransform.rotation, _targetRotation);

        while (Quaternion.Angle(_doorTransform.rotation, _targetRotation) > 0.5f)
        {
            float rotationStep = _rotationSpeed * Time.deltaTime * 100f;
            _doorTransform.rotation = Quaternion.RotateTowards(
                _doorTransform.rotation,
                _targetRotation,
                rotationStep
            );

            float remainingAngle = Quaternion.Angle(_doorTransform.rotation, _targetRotation);
            _closeProgress = 1f - (remainingAngle / totalAngle);
            OnCloseProgress?.Invoke(_closeProgress);

            yield return null;
        }

        _doorTransform.rotation = _targetRotation;
        _closeProgress = 1f;
        _isClosing = false;

        OnCloseProgress?.Invoke(_closeProgress);
        OnCloseCompleted?.Invoke();
    }

    private void PlayCloseSound()
    {
        if (_audioSource != null && _closeSound != null)
        {
            _audioSource.PlayOneShot(_closeSound);
        }
    }

    private void ValidateReferences()
    {
        if (_doorTransform == null)
        {
            _doorTransform = transform;
            Debug.LogWarning("[DoorController] Door transform not set, using this transform");
        }

        if (_rotationSpeed <= 0f)
        {
            Debug.LogError("[DoorController] Rotation speed must be greater than 0", this);
            _rotationSpeed = 2f;
        }

        if (_rotationAxis == Vector3.zero)
        {
            Debug.LogError("[DoorController] Rotation axis cannot be zero", this);
            _rotationAxis = Vector3.up;
        }

        if (_audioSource == null)
        {
            Debug.Log("Audio not assigned", this);
            enabled = false;
            return;
        }
    }

    private void InitializeRotations()
    {
        _initialRotation = _doorTransform.rotation;
        _targetRotation = _initialRotation * Quaternion.AngleAxis(_targetAngle, _rotationAxis.normalized);
    }

    public void ResetDoor()
    {
        StopClosing();
        _doorTransform.rotation = _initialRotation;
        _closeProgress = 0f;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Transform door = _doorTransform != null ? _doorTransform : transform;

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(door.position, door.forward * 2f);

        if (Application.isPlaying && _isClosing)
        {
            Gizmos.color = Color.red;
            Vector3 targetForward = _targetRotation * Vector3.forward;
            Gizmos.DrawRay(door.position, targetForward * 2f);
        }
    }
#endif
}