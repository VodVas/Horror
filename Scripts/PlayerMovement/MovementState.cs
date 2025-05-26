using System;

[Flags]
public enum MovementState : byte
{
    None = 0,
    Grounded = 1 << 0,
    Jumping = 1 << 1,
    Sprinting = 1 << 2,
    InAir = 1 << 3
}