using UnityEngine;

public interface IFootstepController
{
    void UpdateFootsteps(Vector2 movementInput, MovementState state, float deltaTime);
}