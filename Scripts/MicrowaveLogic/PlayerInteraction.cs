//using UnityEngine;
//using Zenject;

//[RequireComponent(typeof(Camera))]
//public sealed class PlayerInteraction : MonoBehaviour
//{
//    [SerializeField] private float _interactionRadius = 2f;
//    [SerializeField] private float _interactionAngle = 45f;
//    [SerializeField] private LayerMask _interactionLayer;
//    [SerializeField] private LayerMask _obstacleLayer;

//    [Inject] private NewInputProvider _inputProvider;

//    private Camera _camera;
//    private Collider[] _colliders = new Collider[32];

//    private void Start() => _camera = GetComponent<Camera>();

//    private void Update()
//    {
//        if (!_inputProvider.GetPushInput()) return;

//        if (TryFindNearestInteractable(out IInteractable interactable))
//            interactable.Interact();
//    }

//    public bool TryFindNearestInteractable(out IInteractable result)
//    {
//        result = null;
//        Vector3 origin = _camera.transform.position;
//        Vector3 direction = _camera.transform.forward;

//        int count = Physics.OverlapSphereNonAlloc(
//            origin,
//            _interactionRadius,
//            _colliders,
//            _interactionLayer
//        );

//        float minSqrDistance = Mathf.Infinity;
//        for (int i = 0; i < count; i++)
//        {
//            Collider col = _colliders[i];

//            if (col == null) continue;

//            Vector3 toCollider = col.transform.position - origin;
//            float sqrDistance = toCollider.sqrMagnitude;
//            float angle = Vector3.Angle(direction, toCollider.normalized);

//            if (angle > _interactionAngle) continue;

//            if (Physics.Raycast(origin, toCollider.normalized,
//                Mathf.Sqrt(sqrDistance), _obstacleLayer)) continue;

//            if (sqrDistance < minSqrDistance &&
//                col.TryGetComponent(out IInteractable interactable))
//            {
//                minSqrDistance = sqrDistance;
//                result = interactable;
//            }
//        }
//        return result != null;
//    }

//    private void OnDrawGizmosSelected()
//    {
//        if (!_camera) return;

//        Gizmos.color = Color.cyan;
//        Gizmos.DrawWireSphere(_camera.transform.position, _interactionRadius);

//        Vector3 origin = _camera.transform.position;
//        Vector3 forward = _camera.transform.forward;
//        Vector3 left = Quaternion.AngleAxis(-_interactionAngle, Vector3.up) * forward;
//        Vector3 right = Quaternion.AngleAxis(_interactionAngle, Vector3.up) * forward;

//        Gizmos.DrawRay(origin, left * _interactionRadius);
//        Gizmos.DrawRay(origin, right * _interactionRadius);
//        Gizmos.DrawRay(origin, forward * _interactionRadius);
//    }
//}











using UnityEngine;
using Zenject;

//public sealed class PlayerInteraction : MonoBehaviour
//{
//    [SerializeField] private float _interactionRadius = 2f;
//    [SerializeField] private float _interactionAngle = 30f;
//    [SerializeField] private LayerMask _interactionLayer;
//    [SerializeField] private LayerMask _obstacleLayer;
//    [SerializeField] private Camera _camera;

//    [Inject] private NewInputProvider _inputProvider;

//    private readonly Collider[] _colliders = new Collider[32];

//    private void Update()
//    {
//        if (!_inputProvider.GetPushInput()) return;

//        if (TryFindNearestInteractable(out IInteractable interactable))
//        {
//            interactable.Interact();
//        }
//    }

//    public bool TryFindNearestInteractable(out IInteractable result)
//    {
//        result = null;
//        Vector3 origin = _camera.transform.position;
//        Vector3 direction = _camera.transform.forward;

//        int count = Physics.OverlapSphereNonAlloc(origin, _interactionRadius, _colliders, _interactionLayer);

//        float minSqrDistance = Mathf.Infinity;
//        for (int i = 0; i < count; i++)
//        {
//            Collider col = _colliders[i];
//            if (col == null) continue;

//            Vector3 toCollider = col.transform.position - origin;
//            float sqrDistance = toCollider.sqrMagnitude;
//            float angle = Vector3.Angle(direction, toCollider.normalized);

//            if (angle > _interactionAngle) continue;
//            if (Physics.Raycast(origin, toCollider.normalized, Mathf.Sqrt(sqrDistance), _obstacleLayer)) continue;
//            //float raycastOffset = 0.2f; // Небольшой отступ от камеры
//            //if (Physics.Raycast(origin + direction * raycastOffset, toCollider, toCollider.magnitude - raycastOffset, _obstacleLayer))
//              //  continue;

//            if (sqrDistance < minSqrDistance && col.TryGetComponent(out IInteractable interactable))
//            {
//                minSqrDistance = sqrDistance;
//                result = interactable;
//            }
//        }
//        return result != null;
//    }

//    private void OnDrawGizmosSelected()
//    {
//        if (!_camera) return;

//        // Полупрозрачная сфера
//        Gizmos.color = new Color(0, 1, 1, 0.2f); // голубой с прозрачностью
//        Gizmos.DrawMesh(_debugSphereMesh, _camera.transform.position);

//        // Лучи угла обзора
//        Vector3 origin = _camera.transform.position;
//        Vector3 forward = _camera.transform.forward;
//        Vector3 left = Quaternion.AngleAxis(-_interactionAngle, Vector3.up) * forward;
//        Vector3 right = Quaternion.AngleAxis(_interactionAngle, Vector3.up) * forward;

//        Gizmos.color = Color.yellow;
//        Gizmos.DrawRay(origin, left * _interactionRadius);
//        Gizmos.DrawRay(origin, right * _interactionRadius);
//        Gizmos.DrawRay(origin, forward * _interactionRadius);
//    }
//}




public sealed class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private DetectionType _detectionType = DetectionType.Raycast;
    [SerializeField] private float _spherecastRadius = 0.5f;
    [SerializeField] private float _interactionRadius = 2f;
    [SerializeField] private float _interactionAngle = 30f;
    [SerializeField] private LayerMask _interactionLayer;
    [SerializeField] private LayerMask _obstacleLayer;
    [SerializeField] private Camera _camera;

    [Inject] private NewInputProvider _inputProvider;

    private readonly Collider[] _colliders = new Collider[302];

    private enum DetectionType { Raycast, Spherecast }

    private void Update()
    {
        if (!_inputProvider.GetPushInput()) return;

        if (TryFindNearestInteractable(out IInteractable interactable))
        {
            interactable.Interact();
        }
    }

    public bool TryFindNearestInteractable(out IInteractable result)
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
            bool hasObstacle = false;

            // Выбор метода обнаружения
            if (_detectionType == DetectionType.Raycast)
            {
                hasObstacle = Physics.Raycast(
                    origin,
                    toCollider.normalized,
                    distance,
                    _obstacleLayer);
            }
            else
            {
                hasObstacle = Physics.SphereCast(
                    origin,
                    _spherecastRadius,
                    toCollider.normalized,
                    out RaycastHit hit,
                    distance,
                    _obstacleLayer);
            }

            if (hasObstacle) continue;

            if (sqrDistance < minSqrDistance && col.TryGetComponent(out IInteractable interactable))
            {
                minSqrDistance = sqrDistance;
                result = interactable;
            }
        }
        return result != null;
    }


    private void OnDrawGizmosSelected()
    {
        if (!_camera) return;

        // Базовая визуализация
        Gizmos.color = new Color(0, 1, 1, 0.2f);
        Gizmos.DrawWireSphere(_camera.transform.position, _interactionRadius);

        // Визуализация угла
        Vector3 origin = _camera.transform.position;
        Vector3 forward = _camera.transform.forward;
        Vector3 left = Quaternion.AngleAxis(-_interactionAngle, Vector3.up) * forward;
        Vector3 right = Quaternion.AngleAxis(_interactionAngle, Vector3.up) * forward;

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(origin, left * _interactionRadius);
        Gizmos.DrawRay(origin, right * _interactionRadius);
        Gizmos.DrawRay(origin, forward * _interactionRadius);

        // Визуализация Spherecast
        if (_detectionType == DetectionType.Spherecast)
        {
            Gizmos.color = Color.magenta;
            Vector3 endPoint = origin + forward * _interactionRadius;

            // Капсула для визуализации
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
