using System;
using UnityEngine;

[Serializable]
public class QuantumEffectParameters : AudioEffectParameters
{
    [Range(0.1f, 10f)] public float modulationRate = 2f;
    [Range(0f, 1f)] public float modulationDepth = 0.5f;
    [Range(0.5f, 2f)] public float timeStretch = 1f;
    [Range(-12f, 12f)] public float pitchShift = 0f;

    public override IAudioEffect CreateEffect()
    {
        return new QuantumEffect(this);
    }
}