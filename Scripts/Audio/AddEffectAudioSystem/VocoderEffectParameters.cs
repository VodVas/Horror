using System;
using UnityEngine;

[Serializable]
public class VocoderEffectParameters : AudioEffectParameters
{
    [Range(2, 32)] public int bandCount = 16;
    [Range(0f, 1f)] public float carrierMix = 0.8f;
    [Range(50f, 500f)] public float carrierFrequency = 100f;

    public override IAudioEffect CreateEffect()
    {
        return new VocoderEffect(this);
    }
}