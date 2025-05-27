using System;
using UnityEngine;

[Serializable]
public class BlendShapeWeight
{
    [field: SerializeField] public BlendShape BlendShape { get; private set; }
    [field: SerializeField, Range(0f, 1f)] public float Weight { get; private set; }

    public BlendShapeWeight(BlendShape blendShape, float weight)
    {
        BlendShape = blendShape; Weight = weight;
    }

    public void SetWeight(float weigh)
    {
        Weight = weigh;
    }
}