using System;

[Serializable]
public abstract class AudioEffectParameters
{
    public abstract IAudioEffect CreateEffect();
}