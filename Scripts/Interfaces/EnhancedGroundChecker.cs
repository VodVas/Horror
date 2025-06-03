using System.Runtime.CompilerServices;
using UnityEngine;

public sealed class EnhancedGroundChecker : IGroundChecker
{
    private const int MAX_RAYCAST_HITS = 5;
    private const float MIN_GROUND_DISTANCE = 0.005f;
    private const float SLOPE_LIMIT = 45f;

    private readonly RaycastHit[] _sphereHitBuffer = new RaycastHit[8];
    private readonly RaycastHit[] _rayHitBuffer = new RaycastHit[MAX_RAYCAST_HITS];

    private bool _isGrounded;
    private Vector3 _groundNormal = Vector3.up;
    private float _groundDistance;
    private Vector3 _lastCheckPosition;
    private Vector3 _groundPoint;

    public bool IsGrounded => _isGrounded;
    public Vector3 GroundNormal => _groundNormal;
    public float GroundDistance => _groundDistance;

    public GroundCheckMethod CheckMethod { get; set; } = GroundCheckMethod.Combined;
    public GroundCheckDebugSettings DebugSettings { get; } = new GroundCheckDebugSettings();

    private struct RaycastOrigin
    {
        public Vector3 point;
        public Vector3 direction;

        public RaycastOrigin(Vector3 p, Vector3 d)
        {
            point = p;
            direction = d;
        }
    }

    public void CheckGround(Vector3 position, float radius, float height, float checkDistance, LayerMask groundMask)
    {
        _lastCheckPosition = position;

        switch (CheckMethod)
        {
            case GroundCheckMethod.SphereCast:
                CheckGroundSphereCast(position, radius, height, checkDistance, groundMask);
                break;
            case GroundCheckMethod.Raycast:
                CheckGroundRaycast(position, radius, height, checkDistance, groundMask);
                break;
            case GroundCheckMethod.Combined:
                CheckGroundCombined(position, radius, height, checkDistance, groundMask);
                break;
        }

        if (DebugSettings.showDebugInfo)
        {
            Debug.Log($"Ground Check: {(_isGrounded ? "GROUNDED" : "AIRBORNE")} | Distance: {_groundDistance:F3} | Normal: {_groundNormal}");
        }
    }

    private void CheckGroundSphereCast(Vector3 position, float radius, float height, float checkDistance, LayerMask groundMask)
    {
        Vector3 sphereBottom = position - Vector3.up * (height * 0.5f - radius);
        float sphereCheckDistance = checkDistance + radius;

        int hitCount = Physics.SphereCastNonAlloc(
            sphereBottom,
            radius * 0.99f,
            Vector3.down,
            _sphereHitBuffer,
            sphereCheckDistance,
            groundMask,
            QueryTriggerInteraction.Ignore
        );

        _isGrounded = false;
        _groundDistance = float.MaxValue;
        _groundNormal = Vector3.up;

        for (int i = 0; i < hitCount; i++)
        {
            RaycastHit hit = _sphereHitBuffer[i];

            if (hit.distance <= MIN_GROUND_DISTANCE)
            {
                float angle = Vector3.Angle(hit.normal, Vector3.up);
                if (angle <= SLOPE_LIMIT)
                {
                    _isGrounded = true;
                    _groundNormal = hit.normal;
                    _groundDistance = hit.distance;
                    _groundPoint = hit.point;
                    break;
                }
            }
        }
    }

    private void CheckGroundRaycast(Vector3 position, float radius, float height, float checkDistance, LayerMask groundMask)
    {
        RaycastOrigin[] origins = GetRaycastOrigins(position, radius, height);

        _isGrounded = false;
        _groundDistance = float.MaxValue;
        _groundNormal = Vector3.zero;
        int validHitCount = 0;

        foreach (var origin in origins)
        {
            int hitCount = Physics.RaycastNonAlloc(
                origin.point,
                origin.direction,
                _rayHitBuffer,
                checkDistance + height * 0.5f,
                groundMask,
                QueryTriggerInteraction.Ignore
            );

            for (int i = 0; i < hitCount; i++)
            {
                RaycastHit hit = _rayHitBuffer[i];
                float adjustedDistance = hit.distance - (height * 0.5f - MIN_GROUND_DISTANCE);

                if (adjustedDistance <= checkDistance)
                {
                    float angle = Vector3.Angle(hit.normal, Vector3.up);
                    if (angle <= SLOPE_LIMIT)
                    {
                        _isGrounded = true;
                        if (adjustedDistance < _groundDistance)
                        {
                            _groundDistance = adjustedDistance;
                            _groundPoint = hit.point;
                        }
                        _groundNormal += hit.normal;
                        validHitCount++;
                    }
                }
            }
        }

        if (validHitCount > 0)
        {
            _groundNormal = (_groundNormal / validHitCount).normalized;
        }
        else
        {
            _groundNormal = Vector3.up;
        }
    }

    private void CheckGroundCombined(Vector3 position, float radius, float height, float checkDistance, LayerMask groundMask)
    {
        CheckGroundSphereCast(position, radius, height, checkDistance, groundMask);

        if (_isGrounded)
        {
            RaycastOrigin[] origins = GetRaycastOrigins(position, radius, height);
            Vector3 avgNormal = Vector3.zero;
            int validHits = 0;

            foreach (var origin in origins)
            {
                if (Physics.Raycast(origin.point, origin.direction, out RaycastHit hit, checkDistance + height * 0.5f, groundMask, QueryTriggerInteraction.Ignore))
                {
                    float angle = Vector3.Angle(hit.normal, Vector3.up);
                    if (angle <= SLOPE_LIMIT)
                    {
                        avgNormal += hit.normal;
                        validHits++;
                    }
                }
            }

            if (validHits > 0)
            {
                _groundNormal = (avgNormal / validHits).normalized;
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private RaycastOrigin[] GetRaycastOrigins(Vector3 position, float radius, float height)
    {
        float offset = radius * 0.7f;
        return new RaycastOrigin[]
        {
            new RaycastOrigin(position, Vector3.down),
            new RaycastOrigin(position + Vector3.forward * offset, Vector3.down),
            new RaycastOrigin(position - Vector3.forward * offset, Vector3.down),
            new RaycastOrigin(position + Vector3.right * offset, Vector3.down),
            new RaycastOrigin(position - Vector3.right * offset, Vector3.down)
        };
    }

    public void DrawDebugVisualization(Vector3 position, float radius, float height, float checkDistance)
    {
        if (!DebugSettings.showGizmos) return;

        Color mainColor = _isGrounded ? DebugSettings.groundedColor : DebugSettings.airborneColor;

        if (CheckMethod == GroundCheckMethod.SphereCast || CheckMethod == GroundCheckMethod.Combined)
        {
            Vector3 sphereBottom = position - Vector3.up * (height * 0.5f - radius);
            Debug.DrawLine(sphereBottom, sphereBottom + Vector3.down * (checkDistance + radius), mainColor);

            DrawWireSphere(sphereBottom, radius * 0.99f, mainColor);
            DrawWireSphere(sphereBottom + Vector3.down * (checkDistance + radius), radius * 0.99f, mainColor * 0.5f);
        }

        if (CheckMethod == GroundCheckMethod.Raycast || CheckMethod == GroundCheckMethod.Combined)
        {
            RaycastOrigin[] origins = GetRaycastOrigins(position, radius, height);
            foreach (var origin in origins)
            {
                Debug.DrawRay(origin.point, origin.direction * (checkDistance + height * 0.5f), DebugSettings.rayColor);
            }
        }

        if (_isGrounded && _groundPoint != Vector3.zero)
        {
            DrawCross(_groundPoint, 0.1f, DebugSettings.hitPointColor);
            Debug.DrawRay(_groundPoint, _groundNormal * 0.5f, Color.cyan);
        }
    }

    private void DrawWireSphere(Vector3 center, float radius, Color color)
    {
        int segments = 16;
        float angleStep = 360f / segments;

        for (int i = 0; i < segments; i++)
        {
            float angle1 = i * angleStep * Mathf.Deg2Rad;
            float angle2 = (i + 1) * angleStep * Mathf.Deg2Rad;

            Vector3 p1 = center + new Vector3(Mathf.Cos(angle1), 0, Mathf.Sin(angle1)) * radius;
            Vector3 p2 = center + new Vector3(Mathf.Cos(angle2), 0, Mathf.Sin(angle2)) * radius;
            Debug.DrawLine(p1, p2, color);

            p1 = center + new Vector3(Mathf.Cos(angle1), Mathf.Sin(angle1), 0) * radius;
            p2 = center + new Vector3(Mathf.Cos(angle2), Mathf.Sin(angle2), 0) * radius;
            Debug.DrawLine(p1, p2, color);

            p1 = center + new Vector3(0, Mathf.Cos(angle1), Mathf.Sin(angle1)) * radius;
            p2 = center + new Vector3(0, Mathf.Cos(angle2), Mathf.Sin(angle2)) * radius;
            Debug.DrawLine(p1, p2, color);
        }
    }

    private void DrawCross(Vector3 center, float size, Color color)
    {
        Debug.DrawLine(center - Vector3.right * size, center + Vector3.right * size, color);
        Debug.DrawLine(center - Vector3.up * size, center + Vector3.up * size, color);
        Debug.DrawLine(center - Vector3.forward * size, center + Vector3.forward * size, color);
    }
}