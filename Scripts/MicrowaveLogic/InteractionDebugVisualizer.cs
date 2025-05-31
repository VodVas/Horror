//using UnityEngine;

//public class InteractionDebugVisualizer
//{
//    private readonly Camera _camera;
//    private readonly float _interactionRadius;
//    private readonly float _interactionAngle;
//    private readonly DetectionType _detectionType;
//    private readonly float _spherecastRadius;

//    public InteractionDebugVisualizer(Camera camera, float interactionRadius, float interactionAngle,
//        DetectionType detectionType, float spherecastRadius)
//    {
//        _camera = camera;
//        _interactionRadius = interactionRadius;
//        _interactionAngle = interactionAngle;
//        _detectionType = detectionType;
//        _spherecastRadius = spherecastRadius;
//    }

//    public void DrawGizmos()
//    {
//        if (!_camera) return;

//        DrawInteractionRange();
//        DrawDetectionRays();

//        if (_detectionType == DetectionType.Spherecast)
//        {
//            DrawSpherecastVisualization();
//        }
//    }

//    private void DrawInteractionRange()
//    {
//        Gizmos.color = new Color(0, 1, 1, 0.2f);
//        Gizmos.DrawWireSphere(_camera.transform.position, _interactionRadius);
//    }

//    private void DrawDetectionRays()
//    {
//        Vector3 origin = _camera.transform.position;
//        Vector3 forward = _camera.transform.forward;
//        Vector3 left = Quaternion.AngleAxis(-_interactionAngle, Vector3.up) * forward;
//        Vector3 right = Quaternion.AngleAxis(_interactionAngle, Vector3.up) * forward;

//        Gizmos.color = Color.yellow;
//        Gizmos.DrawRay(origin, left * _interactionRadius);
//        Gizmos.DrawRay(origin, right * _interactionRadius);
//        Gizmos.DrawRay(origin, forward * _interactionRadius);
//    }

//    private void DrawSpherecastVisualization()
//    {
//        Gizmos.color = Color.magenta;
//        Vector3 origin = _camera.transform.position;
//        Vector3 endPoint = origin + _camera.transform.forward * _interactionRadius;
//        DrawSpherecastGizmo(origin, endPoint, _spherecastRadius);
//    }

//    private void DrawSpherecastGizmo(Vector3 start, Vector3 end, float radius)
//    {
//        Gizmos.DrawWireSphere(start, radius);
//        Gizmos.DrawWireSphere(end, radius);

//        Vector3 direction = (end - start).normalized;
//        Vector3 perpendicular = Vector3.Cross(direction, Vector3.up).normalized * radius;

//        Gizmos.DrawLine(start + perpendicular, end + perpendicular);
//        Gizmos.DrawLine(start - perpendicular, end - perpendicular);

//        perpendicular = Vector3.Cross(direction, Vector3.right).normalized * radius;
//        Gizmos.DrawLine(start + perpendicular, end + perpendicular);
//        Gizmos.DrawLine(start - perpendicular, end - perpendicular);
//    }
//}