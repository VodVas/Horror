using UnityEngine;
using DG.Tweening;
using System;

[Serializable]
public struct FeedbackAnimationData
{
    [Range(0.1f, 2f)] public float riseDuration;
    [Range(0.1f, 1f)] public float fadeDelay;
    [Range(0.1f, 1f)] public float fadeDuration;

    public Vector2 riseOffset;
    public Ease movementEase;
}