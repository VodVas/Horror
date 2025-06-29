using System;
using UnityEngine;
using UnityEngine.Events;

public sealed class WaypointMover : MonoBehaviour
{
    public enum PatrolMode
    {
        Loop,
        PingPong,
        Once
    }

    public enum OrientationMode
    {
        None,
        LookAtDirection,
        FreeRotation,
        SurfaceAlign
    }

    [Header("Waypoints")]
    [SerializeField] private Transform[] _waypoints = Array.Empty<Transform>();
    [SerializeField] private PatrolMode _patrolMode = PatrolMode.Loop;

    [Header("Movement Settings")]
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _rotationSpeed = 720f;
    [SerializeField] private float _stoppingDistance = 0.1f;

    [Header("Orientation Settings")]
    [SerializeField] private OrientationMode _orientationMode = OrientationMode.LookAtDirection;
    [SerializeField] private Vector3 _customUpVector = Vector3.up;
    [SerializeField] private bool _useCustomUpVector = false;
    [SerializeField, Range(0f, 45f)] private float _bankAngle = 0f;
    [SerializeField] private float _bankSpeed = 2f;

    [Header("Surface Alignment")]
    [SerializeField] private float _surfaceCheckDistance = 2f;
    [SerializeField] private LayerMask _surfaceLayerMask = -1;
    [SerializeField] private Vector3 _surfaceCheckDirection = Vector3.down;

    [Header("Events")]
    [SerializeField] private UnityEvent<int> _onWaypointReached = new UnityEvent<int>();
    [SerializeField] private UnityEvent _onPathCompleted = new UnityEvent();

    [Header("Debug")]
    [SerializeField] private bool _showDebugInfo = true;
    [SerializeField] private Color _pathColor = Color.yellow;
    [SerializeField] private float _waypointGizmoSize = 0.5f;

    private int _currentWaypointIndex;
    private bool _isMovingForward = true;
    private bool _isActive = true;
    private Vector3 _lastPosition;
    private float _totalDistanceTraveled;
    private Vector3 _lastMovementDirection;
    private float _currentBankAngle;
    private Quaternion _targetRotation;
    private Vector3 _currentSurfaceNormal = Vector3.up;

    public bool IsMoving { get; private set; }
    public int CurrentWaypointIndex => _currentWaypointIndex;
    public float ProgressToNextWaypoint { get; private set; }
    public float TotalDistanceTraveled => _totalDistanceTraveled;
    public Vector3 CurrentMovementDirection => _lastMovementDirection;

    private void Start()
    {
        ValidateWaypoints();

        if (_waypoints.Length > 0)
        {
            transform.position = _waypoints[0].position;
            _lastPosition = transform.position;
            _targetRotation = transform.rotation;
        }
    }

    private void Update()
    {
        if (!_isActive || _waypoints.Length == 0) return;

        UpdateMovement();
        UpdateDistanceTracking();
    }

    private void ValidateWaypoints()
    {
        if (_waypoints.Length == 0)
        {
            Debug.LogWarning($"No waypoints assigned to {gameObject.name}", this);
            return;
        }

        for (int i = 0; i < _waypoints.Length; i++)
        {
            if (_waypoints[i] == null)
            {
                Debug.LogError($"Waypoint at index {i} is null", this);
            }
        }
    }

    [ContextMenu("UpdateMovement")]
    private void UpdateMovement()
    {
        if (_currentWaypointIndex >= _waypoints.Length) return;

        Transform targetWaypoint = _waypoints[_currentWaypointIndex];
        if (!targetWaypoint) return;

        Vector3 targetPosition = targetWaypoint.position;
        Vector3 direction = targetPosition - transform.position;
        float distanceToTarget = direction.magnitude;

        if (distanceToTarget <= _stoppingDistance)
        {
            OnReachedWaypoint();
            return;
        }

        IsMoving = true;

        // Calculate progress
        if (_currentWaypointIndex > 0)
        {
            Vector3 previousPos = _waypoints[_currentWaypointIndex - 1].position;
            float totalDistance = Vector3.Distance(previousPos, targetPosition);
            ProgressToNextWaypoint = totalDistance > 0 ? 1f - (distanceToTarget / totalDistance) : 1f;
        }
        else
        {
            ProgressToNextWaypoint = 0f;
        }

        // Update rotation based on orientation mode
        if (direction != Vector3.zero && _rotationSpeed > 0 && _orientationMode != OrientationMode.None)
        {
            UpdateOrientation(direction);
        }

        // Movement
        Vector3 movement = direction.normalized * _moveSpeed * Time.deltaTime;
        if (movement.magnitude > distanceToTarget)
        {
            movement = direction;
        }

        transform.position += movement;
        _lastMovementDirection = direction.normalized;
    }

    private void UpdateOrientation(Vector3 movementDirection)
    {
        switch (_orientationMode)
        {
            case OrientationMode.LookAtDirection:
                UpdateLookAtOrientation(movementDirection);
                break;

            case OrientationMode.FreeRotation:
                UpdateFreeRotationOrientation(movementDirection);
                break;

            case OrientationMode.SurfaceAlign:
                UpdateSurfaceAlignedOrientation(movementDirection);
                break;
        }

        // Apply rotation with interpolation
        transform.rotation = Quaternion.Slerp(transform.rotation, _targetRotation, _rotationSpeed * Time.deltaTime / 360f);
    }

    private void UpdateLookAtOrientation(Vector3 movementDirection)
    {
        Vector3 upVector = _useCustomUpVector ? _customUpVector : Vector3.up;
        _targetRotation = Quaternion.LookRotation(movementDirection, upVector);

        // Apply banking effect if enabled
        if (_bankAngle > 0)
        {
            ApplyBankingEffect(movementDirection);
        }
    }

    private void UpdateFreeRotationOrientation(Vector3 movementDirection)
    {
        // Calculate up vector based on movement change
        Vector3 movementDelta = movementDirection - _lastMovementDirection;
        Vector3 upVector = Vector3.up;

        if (movementDelta.magnitude > 0.01f)
        {
            // Use cross product to find perpendicular up vector
            Vector3 right = Vector3.Cross(movementDirection, _lastMovementDirection).normalized;
            if (right.magnitude > 0.01f)
            {
                upVector = Vector3.Cross(right, movementDirection).normalized;
            }
        }
        else if (_useCustomUpVector)
        {
            upVector = _customUpVector;
        }

        _targetRotation = Quaternion.LookRotation(movementDirection, upVector);
    }

    private void UpdateSurfaceAlignedOrientation(Vector3 movementDirection)
    {
        // Perform raycast to find surface normal
        if (Physics.Raycast(transform.position, _surfaceCheckDirection, out RaycastHit hit, _surfaceCheckDistance, _surfaceLayerMask))
        {
            _currentSurfaceNormal = hit.normal;
        }

        // Project movement direction onto surface plane
        Vector3 projectedDirection = Vector3.ProjectOnPlane(movementDirection, _currentSurfaceNormal).normalized;

        if (projectedDirection.magnitude > 0.01f)
        {
            _targetRotation = Quaternion.LookRotation(projectedDirection, _currentSurfaceNormal);
        }
    }

    private void ApplyBankingEffect(Vector3 movementDirection)
    {
        // Calculate turn intensity based on direction change
        float turnIntensity = Vector3.SignedAngle(_lastMovementDirection, movementDirection, Vector3.up);
        float targetBank = Mathf.Clamp(turnIntensity * _bankAngle / 90f, -_bankAngle, _bankAngle);

        // Smooth banking transition
        _currentBankAngle = Mathf.Lerp(_currentBankAngle, targetBank, _bankSpeed * Time.deltaTime);

        // Apply bank rotation
        _targetRotation *= Quaternion.Euler(0, 0, -_currentBankAngle);
    }

    private void UpdateDistanceTracking()
    {
        float frameDistance = Vector3.Distance(transform.position, _lastPosition);
        _totalDistanceTraveled += frameDistance;
        _lastPosition = transform.position;
    }

    private void OnReachedWaypoint()
    {
        IsMoving = false;
        ProgressToNextWaypoint = 1f;

        _onWaypointReached.Invoke(_currentWaypointIndex);

        switch (_patrolMode)
        {
            case PatrolMode.Loop:
                _currentWaypointIndex = (_currentWaypointIndex + 1) % _waypoints.Length;
                break;

            case PatrolMode.PingPong:
                HandlePingPongMode();
                break;

            case PatrolMode.Once:
                if (_currentWaypointIndex < _waypoints.Length - 1)
                {
                    _currentWaypointIndex++;
                }
                else
                {
                    _isActive = false;
                    _onPathCompleted.Invoke();
                }
                break;
        }
    }

    private void HandlePingPongMode()
    {
        if (_isMovingForward)
        {
            if (_currentWaypointIndex < _waypoints.Length - 1)
            {
                _currentWaypointIndex++;
            }
            else
            {
                _isMovingForward = false;
                _currentWaypointIndex = Mathf.Max(0, _currentWaypointIndex - 1);
            }
        }
        else
        {
            if (_currentWaypointIndex > 0)
            {
                _currentWaypointIndex--;
            }
            else
            {
                _isMovingForward = true;
                _currentWaypointIndex = Mathf.Min(_waypoints.Length - 1, _currentWaypointIndex + 1);
            }
        }
    }

    public void SetActive(bool active)
    {
        _isActive = active;
        if (!active) IsMoving = false;
    }

    public void ResetToStart()
    {
        _currentWaypointIndex = 0;
        _isMovingForward = true;
        _isActive = true;
        _totalDistanceTraveled = 0f;
        _currentBankAngle = 0f;

        if (_waypoints.Length > 0 && _waypoints[0])
        {
            transform.position = _waypoints[0].position;
            _lastPosition = transform.position;
        }
    }

    public void SetMoveSpeed(float speed)
    {
        _moveSpeed = Mathf.Max(0f, speed);
    }

    public void SetRotationSpeed(float speed)
    {
        _rotationSpeed = Mathf.Max(0f, speed);
    }

    public void SetOrientationMode(OrientationMode mode)
    {
        _orientationMode = mode;
    }

    public void SetCustomUpVector(Vector3 upVector)
    {
        _customUpVector = upVector.normalized;
        _useCustomUpVector = true;
    }

    public void TeleportToWaypoint(int index)
    {
        if (index < 0 || index >= _waypoints.Length) return;

        _currentWaypointIndex = index;
        if (_waypoints[index])
        {
            transform.position = _waypoints[index].position;
            _lastPosition = transform.position;
        }
    }

    private void OnDrawGizmos()
    {
        if (!_showDebugInfo || _waypoints == null || _waypoints.Length == 0) return;

        // Draw path
        Gizmos.color = _pathColor;
        for (int i = 0; i < _waypoints.Length; i++)
        {
            if (!_waypoints[i]) continue;

            // Draw waypoint
            Gizmos.DrawWireSphere(_waypoints[i].position, _waypointGizmoSize);

            // Draw connection
            int nextIndex = GetNextIndex(i);
            if (nextIndex != -1 && _waypoints[nextIndex])
            {
                Gizmos.DrawLine(_waypoints[i].position, _waypoints[nextIndex].position);
            }
        }

        // Highlight current target
        if (Application.isPlaying && _currentWaypointIndex < _waypoints.Length && _waypoints[_currentWaypointIndex])
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_waypoints[_currentWaypointIndex].position, _waypointGizmoSize * 1.5f);

            // Draw direction
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, _waypoints[_currentWaypointIndex].position);

            // Draw orientation debug
            if (_orientationMode == OrientationMode.SurfaceAlign)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawRay(transform.position, _currentSurfaceNormal * 2f);
                Gizmos.DrawRay(transform.position, _surfaceCheckDirection * _surfaceCheckDistance);
            }
        }
    }

    private int GetNextIndex(int currentIndex)
    {
        switch (_patrolMode)
        {
            case PatrolMode.Loop:
                return (currentIndex + 1) % _waypoints.Length;

            case PatrolMode.PingPong:
                if (_isMovingForward && currentIndex < _waypoints.Length - 1)
                    return currentIndex + 1;
                else if (!_isMovingForward && currentIndex > 0)
                    return currentIndex - 1;
                return -1;

            case PatrolMode.Once:
                if (currentIndex < _waypoints.Length - 1)
                    return currentIndex + 1;
                return -1;

            default:
                return -1;
        }
    }

    private void OnValidate()
    {
        _moveSpeed = Mathf.Max(0f, _moveSpeed);
        _rotationSpeed = Mathf.Max(0f, _rotationSpeed);
        _stoppingDistance = Mathf.Max(0.01f, _stoppingDistance);
        _bankAngle = Mathf.Clamp(_bankAngle, 0f, 45f);
        _bankSpeed = Mathf.Max(0.1f, _bankSpeed);
        _surfaceCheckDistance = Mathf.Max(0.1f, _surfaceCheckDistance);

        if (_customUpVector == Vector3.zero)
        {
            _customUpVector = Vector3.up;
        }
    }
}