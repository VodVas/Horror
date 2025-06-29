using System;
using UnityEngine;

[Serializable]
public class HorrorEffectParameters : AudioEffectParameters
{
    [Range(0f, 1f)] public float glitchProbability = 0.1f;
    [Range(0f, 1f)] public float reverseChance = 0.05f;
    [Range(-12f, 12f)] public float pitchShiftRange = 3f;
    [Range(0f, 1f)] public float whisperMix = 0.2f;

    public override IAudioEffect CreateEffect()
    {
        return new HorrorEffect(this);
    }
}