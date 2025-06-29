using System;
using UnityEngine;

[Serializable]
public class UnderwaterEffectParameters : AudioEffectParameters
{
    [Range(200f, 2000f)] public float cutoffFrequency = 800f;
    [Range(0f, 1f)] public float echoDelay = 0.1f;
    [Range(0f, 0.9f)] public float echoDecay = 0.5f;
    [Range(0f, 1f)] public float wobbleAmount = 0.3f;

    public override IAudioEffect CreateEffect()
    {
        return new UnderwaterEffect(this);
    }
}