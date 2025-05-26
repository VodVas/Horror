using UnityEngine;

[CreateAssetMenu(fileName = "MovementConfig", menuName = "ScriptableObjects/MovementSystem/MovementConfig", order = 1)]
public class MovementConfig : ScriptableObject
{
    [field: SerializeField] public float WalkSpeed { get; private set; } = 5f;
    [field: SerializeField] public float SprintSpeed { get; private set; } = 8f;
    [field: SerializeField] public float JumpForce { get; private set; } = 5f;
    [field: SerializeField] public float Gravity { get; private set; } = 20f;
    [field: SerializeField] public float MovementSharpness { get; private set; } = 15f;
    [field: SerializeField] public float AccelerationInAir { get; private set; } = 5f;
    [field: SerializeField] public float AirSpeedLimit { get; private set; } = 1.2f;
    [field: SerializeField] public float MouseSensitivity { get; private set; } = 100f;
    [field: SerializeField] public float MaxVerticalAngle { get; private set; } = 80f;
    [field: SerializeField] public float GroundCheckDistance { get; private set; } = 0.1f;
    [field: SerializeField] public float GroundCheckRadiusMultiplier { get; private set; } = 0.9f;
    [field: SerializeField] public float JumpGroundingPreventionTime { get; private set; } = 0.2f;
    [field: SerializeField] public float GamepadLookSensitivity { get; private set; } = 300.0f;
    [field: SerializeField] public bool CanJump { get; private set; } = true;
    [field: SerializeField] public AudioClip[] FootstepClips { get; private set; }
    [field: SerializeField] public float WalkStepInterval { get; private set; } = 0.5f;
    [field: SerializeField] public float SprintStepInterval{ get; private set; } = 0.3f;
    [field: SerializeField, Range(0, 1)] public float FootstepVolume{ get; private set; } = 0.8f;
    [field: SerializeField] public Vector2 PitchRange{ get; private set; } = new(0.9f, 1.1f);
}