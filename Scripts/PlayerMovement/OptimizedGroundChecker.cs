using UnityEngine;

public sealed class OptimizedGroundChecker : IGroundChecker
{
    private static readonly RaycastHit[] HitBuffer = new RaycastHit[8];

    private static readonly Vector3[] RayDirections = {
        Vector3.down,
        new Vector3(0.5f, -1f, 0).normalized,
        new Vector3(-0.5f, -1f, 0).normalized,
        new Vector3(0, -1f, 0.5f).normalized,
        new Vector3(0, -1f, -0.5f).normalized
    };

    private bool _isGrounded;
    private Vector3 _groundNormal;
    private float _lastCheckTime;
    private const float CHECK_INTERVAL = 0.02f;

    public bool IsGrounded => _isGrounded;
    public Vector3 GroundNormal => _groundNormal;

    public void CheckGround(Vector3 position, float radius, float height, float checkDistance, LayerMask groundMask)
    {
        if (Time.time - _lastCheckTime < CHECK_INTERVAL) return;
        _lastCheckTime = Time.time;

        Vector3 sphereCenter = position + Vector3.down * (height * 0.5f - radius + 0.02f);

        int hitCount = Physics.SphereCastNonAlloc(
            sphereCenter,
            radius * 0.95f,
            Vector3.down,
            HitBuffer,
            checkDistance,
            groundMask,
            QueryTriggerInteraction.Ignore
        );

        _isGrounded = false;
        _groundNormal = Vector3.up;

        for (int i = 0; i < hitCount; i++)
        {
            float angle = Vector3.Angle(HitBuffer[i].normal, Vector3.up);
            if (angle <= 45f)
            {
                _isGrounded = true;
                _groundNormal = HitBuffer[i].normal;
                break;
            }
        }
    }
}