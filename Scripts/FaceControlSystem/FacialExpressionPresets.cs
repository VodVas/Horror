using UnityEngine;

public static class FacialExpressionPresets
{
    public static EmotionData CreateHappyEmotion()
    {
        var data = ScriptableObject.CreateInstance<EmotionData>();
        return data;
    }

    public static BlendShapeWeight[] HappyExpression => new BlendShapeWeight[]
    {
        new BlendShapeWeight(BlendShape.MouthSmileLeft, 0.8f),
        new BlendShapeWeight(BlendShape.MouthSmileRight, 0.8f),
        new BlendShapeWeight(BlendShape.CheekSquintLeft, 0.4f),
        new BlendShapeWeight(BlendShape.CheekSquintRight, 0.4f),
        new BlendShapeWeight(BlendShape.EyeSquintLeft, 0.2f),
        new BlendShapeWeight(BlendShape.EyeSquintRight, 0.2f)
    };

    public static BlendShapeWeight[] SadExpression => new BlendShapeWeight[]
    {
        new BlendShapeWeight(BlendShape.MouthFrownLeft, 0.7f),
        new BlendShapeWeight(BlendShape.MouthFrownRight, 0.7f),
        new BlendShapeWeight(BlendShape.BrowInnerUp, 0.6f),
        new BlendShapeWeight(BlendShape.MouthLowerDownLeft, 0.3f),
        new BlendShapeWeight(BlendShape.MouthLowerDownRight, 0.3f)
    };

    public static BlendShapeWeight[] AngryExpression => new BlendShapeWeight[]
    {
        new BlendShapeWeight(BlendShape.BrowDownLeft, 0.8f),
        new BlendShapeWeight(BlendShape.BrowDownRight, 0.8f),
        new BlendShapeWeight(BlendShape.EyeSquintLeft, 0.6f),
        new BlendShapeWeight(BlendShape.EyeSquintRight, 0.6f),
        new BlendShapeWeight(BlendShape.NoseSneerLeft, 0.4f),
        new BlendShapeWeight(BlendShape.NoseSneerRight, 0.4f),
        new BlendShapeWeight(BlendShape.MouthPressLeft, 0.5f),
        new BlendShapeWeight(BlendShape.MouthPressRight, 0.5f)
    };

    public static BlendShapeWeight[] SurprisedExpression => new BlendShapeWeight[]
    {
        new BlendShapeWeight(BlendShape.BrowOuterUpLeft, 0.9f),
        new BlendShapeWeight(BlendShape.BrowOuterUpRight, 0.9f),
        new BlendShapeWeight(BlendShape.EyeWideLeft, 0.8f),
        new BlendShapeWeight(BlendShape.EyeWideRight, 0.8f),
        new BlendShapeWeight(BlendShape.JawOpen, 0.5f),
        new BlendShapeWeight(BlendShape.MouthFunnel, 0.3f)
    };

    public static BlendShapeWeight[] DisgustedExpression => new BlendShapeWeight[]
    {
        new BlendShapeWeight(BlendShape.NoseSneerLeft, 0.9f),
        new BlendShapeWeight(BlendShape.NoseSneerRight, 0.9f),
        new BlendShapeWeight(BlendShape.MouthUpperUpLeft, 0.6f),
        new BlendShapeWeight(BlendShape.MouthUpperUpRight, 0.6f),
        new BlendShapeWeight(BlendShape.CheekSquintLeft, 0.7f),
        new BlendShapeWeight(BlendShape.CheekSquintRight, 0.7f)
    };

    public static BlendShapeWeight[] FearfulExpression => new BlendShapeWeight[]
    {
        new BlendShapeWeight(BlendShape.EyeWideLeft, 1f),
        new BlendShapeWeight(BlendShape.EyeWideRight, 1f),
        new BlendShapeWeight(BlendShape.BrowOuterUpLeft, 0.8f),
        new BlendShapeWeight(BlendShape.BrowOuterUpRight, 0.8f),
        new BlendShapeWeight(BlendShape.JawOpen, 0.4f),
        new BlendShapeWeight(BlendShape.MouthStretchLeft, 0.5f),
        new BlendShapeWeight(BlendShape.MouthStretchRight, 0.5f)
    };

    public static BlendShapeWeight[] ConfusedExpression => new BlendShapeWeight[]
    {
    new BlendShapeWeight(BlendShape.BrowInnerUp, 0.7f),
    new BlendShapeWeight(BlendShape.BrowOuterUpLeft, 0.5f),
    new BlendShapeWeight(BlendShape.BrowOuterUpRight, 0.3f),
    new BlendShapeWeight(BlendShape.MouthFrownLeft, 0.4f),
    new BlendShapeWeight(BlendShape.MouthDimpleRight, 0.4f),
    new BlendShapeWeight(BlendShape.EyeLookOutLeft, 0.3f)
    };

    public static BlendShapeWeight[] HopefulExpression => new[]
    {
    new BlendShapeWeight(BlendShape.BrowInnerUp, 0.6f),
    new BlendShapeWeight(BlendShape.MouthSmileLeft, 0.4f),
    new BlendShapeWeight(BlendShape.MouthSmileRight, 0.4f),
    new BlendShapeWeight(BlendShape.EyeSquintLeft, 0.3f),
    new BlendShapeWeight(BlendShape.EyeSquintRight, 0.3f),
    new BlendShapeWeight(BlendShape.MouthUpperUpLeft, 0.2f),
    new BlendShapeWeight(BlendShape.MouthUpperUpRight, 0.2f)
    };

    public static BlendShapeWeight[] GratefulExpression => new[]
    {
    new BlendShapeWeight(BlendShape.MouthSmileLeft, 0.7f),
    new BlendShapeWeight(BlendShape.MouthSmileRight, 0.5f),
    new BlendShapeWeight(BlendShape.BrowOuterUpLeft, -0.4f),
    new BlendShapeWeight(BlendShape.BrowOuterUpRight, -0.4f),
    new BlendShapeWeight(BlendShape.JawForward, 0.3f),
    new BlendShapeWeight(BlendShape.EyeSquintLeft, 0.2f),
    new BlendShapeWeight(BlendShape.EyeSquintRight, 0.2f)
    };

    public static BlendShapeWeight[] JealousExpression => new[]
    {
    new BlendShapeWeight(BlendShape.EyeSquintLeft, 0.8f),
    new BlendShapeWeight(BlendShape.EyeSquintRight, 0.8f),
    new BlendShapeWeight(BlendShape.MouthPressLeft, 0.6f),
    new BlendShapeWeight(BlendShape.MouthPressRight, 0.6f),
    new BlendShapeWeight(BlendShape.NoseSneerLeft, 0.5f),
    new BlendShapeWeight(BlendShape.NoseSneerRight, 0.5f),
    new BlendShapeWeight(BlendShape.BrowOuterUpLeft, 0.4f)
    };

    public static BlendShapeWeight[] BoredExpression => new[]
    {
    new BlendShapeWeight(BlendShape.EyeLookDownLeft, 0.7f),
    new BlendShapeWeight(BlendShape.EyeLookDownRight, 0.7f),
    new BlendShapeWeight(BlendShape.JawOpen, 0.3f),
    new BlendShapeWeight(BlendShape.MouthFrownLeft, 0.4f),
    new BlendShapeWeight(BlendShape.MouthFrownRight, 0.4f),
    new BlendShapeWeight(BlendShape.MouthStretchLeft, 0.2f),
    new BlendShapeWeight(BlendShape.MouthStretchRight, 0.2f)
    };

    public static BlendShapeWeight[] EcstaticJoyExpression => new[]
    {
    new BlendShapeWeight(BlendShape.MouthSmileLeft, 1.0f),
    new BlendShapeWeight(BlendShape.MouthSmileRight, 1.0f),
    new BlendShapeWeight(BlendShape.CheekSquintLeft, 0.9f),
    new BlendShapeWeight(BlendShape.CheekSquintRight, 0.9f),
    new BlendShapeWeight(BlendShape.EyeWideLeft, 0.8f),
    new BlendShapeWeight(BlendShape.EyeWideRight, 0.8f),
    new BlendShapeWeight(BlendShape.BrowInnerUp, 0.7f),
    new BlendShapeWeight(BlendShape.BrowOuterUpLeft, 0.6f),
    new BlendShapeWeight(BlendShape.BrowOuterUpRight, 0.6f),
    new BlendShapeWeight(BlendShape.JawOpen, 0.7f),
    new BlendShapeWeight(BlendShape.MouthUpperUpLeft, 0.5f),
    new BlendShapeWeight(BlendShape.MouthUpperUpRight, 0.5f),
    new BlendShapeWeight(BlendShape.NoseSneerLeft, 0.3f),
    new BlendShapeWeight(BlendShape.EyeSquintLeft, 0.4f)
    };

    public static BlendShapeWeight[] CosmicHorrorExpression => new[]
    {
    new BlendShapeWeight(BlendShape.BrowInnerUp, 1.0f),
    new BlendShapeWeight(BlendShape.BrowOuterUpLeft, 0.9f),
    new BlendShapeWeight(BlendShape.BrowOuterUpRight, 0.9f),
    new BlendShapeWeight(BlendShape.EyeWideLeft, 1.2f),
    new BlendShapeWeight(BlendShape.EyeWideRight, 1.2f),
    new BlendShapeWeight(BlendShape.EyeSquintLeft, 0.8f),
    new BlendShapeWeight(BlendShape.NoseSneerLeft, 0.9f),
    new BlendShapeWeight(BlendShape.NoseSneerRight, 0.6f),
    new BlendShapeWeight(BlendShape.MouthStretchLeft, 0.7f),
    new BlendShapeWeight(BlendShape.MouthStretchRight, -0.5f),
    new BlendShapeWeight(BlendShape.JawOpen, 0.8f),
    new BlendShapeWeight(BlendShape.MouthLowerDownLeft, 0.6f),
    new BlendShapeWeight(BlendShape.CheekSquintLeft, 0.7f),
    new BlendShapeWeight(BlendShape.MouthRollUpper, 0.4f),
    new BlendShapeWeight(BlendShape.EyeLookOutLeft, 0.9f),
    new BlendShapeWeight(BlendShape.JawForward, 0.3f)
    };
}