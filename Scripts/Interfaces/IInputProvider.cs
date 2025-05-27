using UnityEngine;

public interface IInputProvider
{
    Vector2 GetMovementInput();
    Vector2 GetLookInput();
    bool GetJumpInput();
    bool GetSprintInput();
    bool GetPushInput();

    bool IsUsingGamepad { get; }
}