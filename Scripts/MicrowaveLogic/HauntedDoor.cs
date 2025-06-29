using UnityEngine;
using DG.Tweening;
using System.Collections;

public sealed class HauntedDoor : MonoBehaviour
{
    private enum RotationMode
    {
        LocalAxis,
        CustomAxis,
        RelativeToStart
    }

    [Header("Door Settings")]
    [SerializeField] private Transform _doorTransform;
    [SerializeField] private float _maxOpenAngle = 90f;
    [SerializeField] private float _minOpenAngle = 10f;

    [Header("Rotation Settings")]
    [SerializeField] private RotationMode _rotationMode = RotationMode.CustomAxis;
    [SerializeField] private Vector3 _rotationAxis = Vector3.up;
    [SerializeField] private bool _inverseRotation = false;

    [Header("Animation Settings")]
    [SerializeField] private float _baseAnimationDuration = 1f;
    [SerializeField] private float _pauseBetweenCycles = 0.5f;
    [SerializeField] private AnimationCurve _openCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private AnimationCurve _closeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Randomization")]
    [SerializeField] private bool _randomizeDuration = true;
    [SerializeField] private float _durationRandomRange = 0.3f;

    [Header("Debug")]
    [SerializeField] private bool _showGizmos = true;
    [SerializeField] private float _gizmoLength = 1f;

    private Quaternion _closedRotation;
    private Vector3 _normalizedAxis;
    private Vector3 _startForward;
    private Vector3 _startUp;
    private Tweener _currentTween;
    private Coroutine _hauntingCoroutine;
    private bool _isHaunting;

    private void Awake()
    {
        ValidateReferences();
        CacheInitialState();
    }

    //private void OnEnable() => StartHaunting();
    private void OnDisable() => StopHaunting();
    private void OnDestroy() => CleanupTweens();

    public void SetDuration(float value) => _baseAnimationDuration = Mathf.Max(0.1f, value);
    public void SetPauseDuration(float value) => _pauseBetweenCycles = Mathf.Max(0f, value);

    private void ValidateReferences()
    {
        if (!_doorTransform) _doorTransform = transform;

        _maxOpenAngle = Mathf.Clamp(_maxOpenAngle, 0f, 180f);
        _minOpenAngle = Mathf.Clamp(_minOpenAngle, 0f, _maxOpenAngle);
        _baseAnimationDuration = Mathf.Max(0.1f, _baseAnimationDuration);
        _pauseBetweenCycles = Mathf.Max(0f, _pauseBetweenCycles);
        _durationRandomRange = Mathf.Clamp(_durationRandomRange, 0f, _baseAnimationDuration * 0.5f);

        if (_rotationAxis == Vector3.zero) _rotationAxis = Vector3.up;
        _normalizedAxis = _rotationAxis.normalized;
    }

    private void CacheInitialState()
    {
        _closedRotation = _doorTransform.localRotation;
        _startForward = _doorTransform.forward;
        _startUp = _doorTransform.up;

        if (_rotationMode == RotationMode.CustomAxis)
        {
            _normalizedAxis = _doorTransform.InverseTransformDirection(_rotationAxis.normalized);
        }
    }

    [ContextMenu("StartHaunting")]
    public void StartHaunting()
    {
        if (_isHaunting) return;
        _isHaunting = true;
        _hauntingCoroutine = StartCoroutine(HauntingCycle());
    }

    public void StopHaunting()
    {
        _isHaunting = false;
        if (_hauntingCoroutine != null) StopCoroutine(_hauntingCoroutine);
        CleanupTweens();
        ResetDoorPosition();
    }

    private IEnumerator HauntingCycle()
    {
        while (_isHaunting)
        {
            float targetAngle = Random.Range(_minOpenAngle, _maxOpenAngle);
            yield return AnimateDoor(targetAngle, GetRandomizedDuration(), _openCurve);
            if (_pauseBetweenCycles > 0) yield return new WaitForSeconds(_pauseBetweenCycles * 0.5f);
            yield return AnimateDoor(0f, GetRandomizedDuration(), _closeCurve);
            if (_pauseBetweenCycles > 0) yield return new WaitForSeconds(_pauseBetweenCycles);
        }
    }

    private IEnumerator AnimateDoor(float targetAngle, float duration, AnimationCurve curve)
    {
        CleanupTweens();

        Quaternion targetRotation = CalculateTargetRotation(targetAngle);

        _currentTween = _doorTransform
            .DORotateQuaternion(targetRotation, duration)
            .SetEase(curve);

        yield return _currentTween.WaitForCompletion();
    }

    private Quaternion CalculateTargetRotation(float angle)
    {
        if (_inverseRotation) angle = -angle;

        switch (_rotationMode)
        {
            case RotationMode.LocalAxis:
                return _closedRotation * Quaternion.Euler(0, angle, 0);

            case RotationMode.CustomAxis:
                return _closedRotation * Quaternion.AngleAxis(angle, _normalizedAxis);

            case RotationMode.RelativeToStart:
                Vector3 localUp = _doorTransform.parent ?
                    _doorTransform.parent.InverseTransformDirection(_startUp) : _startUp;
                return _closedRotation * Quaternion.AngleAxis(angle, localUp);

            default:
                return _closedRotation;
        }
    }

    private float GetRandomizedDuration() => _randomizeDuration
        ? Mathf.Max(0.1f, _baseAnimationDuration + Random.Range(-_durationRandomRange, _durationRandomRange))
        : _baseAnimationDuration;

    private void CleanupTweens()
    {
        if (_currentTween?.IsActive() == true) _currentTween.Kill(false);
        _currentTween = null;
    }

    private void ResetDoorPosition()
    {
        if (_doorTransform) _doorTransform.localRotation = _closedRotation;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!_showGizmos || !_doorTransform) return;

        Gizmos.color = Color.yellow;
        Vector3 worldAxis = (_rotationMode == RotationMode.CustomAxis) ?
            _doorTransform.TransformDirection(_normalizedAxis) :
            _doorTransform.up;

        Gizmos.DrawLine(_doorTransform.position, _doorTransform.position + worldAxis * _gizmoLength);

        Gizmos.color = Color.green;
        Vector3 openDirection = Quaternion.AngleAxis(_maxOpenAngle, worldAxis) * _doorTransform.forward;
        Gizmos.DrawLine(_doorTransform.position, _doorTransform.position + openDirection * _gizmoLength * 0.8f);

        Gizmos.color = new Color(0f, 1f, 0f, 0.5f);
        Vector3 minOpenDirection = Quaternion.AngleAxis(_minOpenAngle, worldAxis) * _doorTransform.forward;
        Gizmos.DrawLine(_doorTransform.position, _doorTransform.position + minOpenDirection * _gizmoLength * 0.8f);
    }

    private void OnValidate()
    {
        ValidateReferences();
    }
#endif
}