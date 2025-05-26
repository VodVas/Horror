using System;
using UnityEngine;

[Serializable]
public class BlendShapeWeight
{
    public BlendShape blendShape;
    [Range(0f, 1f)] public float weight;

    public BlendShapeWeight(BlendShape bs, float w) { blendShape = bs; weight = w; }
}
