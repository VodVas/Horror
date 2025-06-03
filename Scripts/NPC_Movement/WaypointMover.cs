using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public enum PatrolMode
{
    Loop,
    PingPong,
    Once
}

public sealed class WaypointMover : MonoBehaviour
{
    [Header("Waypoints")]
    [SerializeField] private Transform[] _waypoints = Array.Empty<Transform>();
    [SerializeField] private PatrolMode _patrolMode = PatrolMode.Loop;

    [Header("Movement Settings")]
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _rotationSpeed = 720f;
    [SerializeField] private float _stoppingDistance = 0.1f;

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

    public bool IsMoving { get; private set; }
    public int CurrentWaypointIndex => _currentWaypointIndex;
    public float ProgressToNextWaypoint { get; private set; }
    public float TotalDistanceTraveled => _totalDistanceTraveled;

    private void Start()
    {
        ValidateWaypoints();
        if (_waypoints.Length > 0)
        {
            transform.position = _waypoints[0].position;
            _lastPosition = transform.position;
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
        ProgressToNextWaypoint = 1f - (distanceToTarget / Vector3.Distance(_waypoints[Mathf.Max(0, _currentWaypointIndex - 1)].position, targetPosition));

        // Rotation
        if (direction != Vector3.zero && _rotationSpeed > 0)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime / 360f);
        }

        // Movement
        Vector3 movement = direction.normalized * _moveSpeed * Time.deltaTime;
        if (movement.magnitude > distanceToTarget)
        {
            movement = direction;
        }

        transform.position += movement;
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
                _currentWaypointIndex--;
                if (_currentWaypointIndex < 0) _currentWaypointIndex = 0;
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
                _currentWaypointIndex++;
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
        }
    }

    private int GetNextIndex(int currentIndex)
    {
        switch (_patrolMode)
        {
            case PatrolMode.Loop:
                return (currentIndex + 1) % _waypoints.Length;

            case PatrolMode.PingPong:
                if (currentIndex < _waypoints.Length - 1)
                    return currentIndex + 1;
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
    }
}
