using System;

public interface IFacialExpressionController
{
    void SetEmotion(EmotionType emotion);
    void SetEmotion(EmotionType emotion, float intensity = 1f, float duration = 0.3f);
    void SetBlendShape(BlendShape blendShape, float weight, float duration = 0f);
    void SetCustomExpression(BlendShapeWeight[] weights, float duration = 0.3f);
    void BlendEmotions(EmotionType primary, EmotionType secondary, float blend, float duration = 0.3f);
    void ResetToNeutral(float duration = 0.3f);
    float GetBlendShapeWeight(BlendShape blendShape);
    bool IsTransitioning { get; }

    event Action<EmotionType> OnEmotionChanged;
}