using System;

public interface ITerminatable
{
    public event Action<ITerminatable> Terminated;
}