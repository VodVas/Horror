using UnityEngine;

public interface IGroundChecker
{
    bool IsGrounded { get; }
    Vector3 GroundNormal { get; }
    void CheckGround(Vector3 position, float radius, float height, float checkDistance, LayerMask mask);
}