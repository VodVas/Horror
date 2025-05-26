using UnityEngine;

public interface IMovementController
{
    Vector3 Velocity { get; }
    void UpdateMovement(Vector2 input, MovementState state, float deltaTime);
    void ApplyGravity(float deltaTime);
    void Jump(float force);
}