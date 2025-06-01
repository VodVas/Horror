using UnityEngine;

public sealed class InteractionTargetDetector
{
    private readonly LayerMask _obstacleLayer;
    private readonly float _interactionAngle;

    public InteractionTargetDetector(LayerMask obstacleLayer, float interactionAngle)
    {
        _obstacleLayer = obstacleLayer;
        _interactionAngle = interactionAngle;
    }

    public bool IsValidTarget(Vector3 origin, Vector3 direction, Vector3 targetPosition)
    {
        Vector3 toTarget = targetPosition - origin;
        float angle = Vector3.Angle(direction, toTarget.normalized);

        if (angle > _interactionAngle)
            return false;

        return !Physics.Linecast(origin, targetPosition, _obstacleLayer);
    }
}
