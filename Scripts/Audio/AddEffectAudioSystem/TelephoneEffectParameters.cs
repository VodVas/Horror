using System;
using UnityEngine;

[Serializable]
public class TelephoneEffectParameters : AudioEffectParameters
{
    [Range(300f, 1000f)] public float lowFrequency = 300f;
    [Range(2000f, 4000f)] public float highFrequency = 3400f;
    [Range(0f, 1f)] public float noiseLevel = 0.05f;
    [Range(0f, 1f)] public float distortion = 0.1f;

    public override IAudioEffect CreateEffect()
    {
        return new TelephoneEffect(this);
    }
}