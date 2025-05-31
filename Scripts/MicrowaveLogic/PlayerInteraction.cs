using UnityEngine;
using Zenject;

public sealed class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private DetectionType _detectionType = DetectionType.Raycast;
    [SerializeField] private float _spherecastRadius = 0.5f;
    [SerializeField] private float _interactionRadius = 2f;
    [SerializeField] private float _interactionAngle = 30f;
    [SerializeField] private LayerMask _interactionLayer;
    [SerializeField] private LayerMask _obstacleLayer;
    [SerializeField] private Camera _camera;
    [SerializeField] private HeldItemManager _heldItemManager;

    private enum DetectionType { Raycast , Spherecast };

    [Inject] private NewInputProvider _inputProvider;

    private readonly Collider[] _colliders = new Collider[32];
    private IInteractable[] _playerInteractables;

    private void Awake()
    {
        _playerInteractables = GetComponentsInChildren<IInteractable>();
    }

    private void Update()
    {
        if (!_inputProvider.GetPushInput()) return;

        if (_heldItemManager.HasItem)
        {
            if (TryFindPlayerInteractable(out IInteractable thrower) && thrower is ItemThrower)
            {
                thrower.Interact();
                return;
            }
        }
        else
        {
            if (TryFindNearestWorldInteractable(out IInteractable worldInteractable))
            {
                worldInteractable.Interact();
                return;
            }
        }

        if (TryFindPlayerInteractable(out IInteractable fallbackInteractable))
        {
            fallbackInteractable.Interact();
        }
    }

    private bool TryFindPlayerInteractable(out IInteractable result)
    {
        result = null;

        foreach (var interactable in _playerInteractables)
        {
            if (interactable?.CanInteract == true)
            {
                result = interactable;
                return true;
            }
        }

        return false;
    }

    public bool TryFindNearestWorldInteractable(out IInteractable result)
    {
        result = null;
        Vector3 origin = _camera.transform.position;
        Vector3 direction = _camera.transform.forward;

        int count = Physics.OverlapSphereNonAlloc(origin, _interactionRadius, _colliders, _interactionLayer);
        float minSqrDistance = Mathf.Infinity;

        for (int i = 0; i < count; i++)
        {
            Collider col = _colliders[i];
            if (col == null) continue;

            Vector3 toCollider = col.transform.position - origin;
            float sqrDistance = toCollider.sqrMagnitude;
            float angle = Vector3.Angle(direction, toCollider.normalized);

            if (angle > _interactionAngle) continue;

            float distance = Mathf.Sqrt(sqrDistance);
            bool hasObstacle = CheckForObstacle(origin, toCollider.normalized, distance);

            if (hasObstacle) continue;

            if (sqrDistance < minSqrDistance &&
                col.TryGetComponent(out IInteractable interactable) &&
                interactable.CanInteract)
            {
                minSqrDistance = sqrDistance;
                result = interactable;
            }
        }

        return result != null;
    }

    private bool CheckForObstacle(Vector3 origin, Vector3 direction, float distance)
    {
        return _detectionType == DetectionType.Raycast
            ? Physics.Raycast(origin, direction, distance, _obstacleLayer)
            : Physics.SphereCast(origin, _spherecastRadius, direction, out _, distance, _obstacleLayer);
    }

    private void OnDrawGizmosSelected()
    {
        if (!_camera) return;

        Gizmos.color = new Color(0, 1, 1, 0.2f);
        Gizmos.DrawWireSphere(_camera.transform.position, _interactionRadius);

        Vector3 origin = _camera.transform.position;
        Vector3 forward = _camera.transform.forward;
        Vector3 left = Quaternion.AngleAxis(-_interactionAngle, Vector3.up) * forward;
        Vector3 right = Quaternion.AngleAxis(_interactionAngle, Vector3.up) * forward;

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(origin, left * _interactionRadius);
        Gizmos.DrawRay(origin, right * _interactionRadius);
        Gizmos.DrawRay(origin, forward * _interactionRadius);

        if (_detectionType == DetectionType.Spherecast)
        {
            Gizmos.color = Color.magenta;
            Vector3 endPoint = origin + forward * _interactionRadius;
            DrawSpherecastGizmo(origin, endPoint, _spherecastRadius);
        }
    }

    private void DrawSpherecastGizmo(Vector3 start, Vector3 end, float radius)
    {
        Gizmos.DrawWireSphere(start, radius);
        Gizmos.DrawWireSphere(end, radius);

        Vector3 direction = (end - start).normalized;
        Vector3 perpendicular = Vector3.Cross(direction, Vector3.up).normalized * radius;

        Gizmos.DrawLine(start + perpendicular, end + perpendicular);
        Gizmos.DrawLine(start - perpendicular, end - perpendicular);

        perpendicular = Vector3.Cross(direction, Vector3.right).normalized * radius;
        Gizmos.DrawLine(start + perpendicular, end + perpendicular);
        Gizmos.DrawLine(start - perpendicular, end - perpendicular);
    }
}
