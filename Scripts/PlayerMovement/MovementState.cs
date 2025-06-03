using System;

[Flags]
public enum MovementState : byte
{
    None = 0,
    Grounded = 1 << 0,
    InAir = 1 << 1,
    Jumping = 1 << 2,
    Sprinting = 1 << 3
}