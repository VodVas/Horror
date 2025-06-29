using System;
using UnityEngine;

[Serializable]
public class RadioEffectParameters : AudioEffectParameters
{
    [Range(0f, 1f)] public float noiseLevel = 0.1f;
    [Range(200f, 4000f)] public float lowPassFrequency = 2000f;
    [Range(100f, 1000f)] public float highPassFrequency = 300f;
    [Range(0f, 1f)] public float distortion = 0.2f;

    public override IAudioEffect CreateEffect()
    {
        return new RadioEffect(this);
    }
}