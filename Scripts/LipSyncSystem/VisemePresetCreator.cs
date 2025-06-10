public static class VisemePresetCreator
{
    public static BlendShapeWeight[] CreateViseme_AA() => new[]
    {
        new BlendShapeWeight(BlendShape.JawOpen, 0.7f),
        new BlendShapeWeight(BlendShape.MouthLowerDownLeft, 0.5f),
        new BlendShapeWeight(BlendShape.MouthLowerDownRight, 0.5f),
        new BlendShapeWeight(BlendShape.MouthStretchLeft, 0.3f),
        new BlendShapeWeight(BlendShape.MouthStretchRight, 0.3f)
    };

    public static BlendShapeWeight[] CreateViseme_EE() => new[]
    {
        new BlendShapeWeight(BlendShape.MouthSmileLeft, 0.6f),
        new BlendShapeWeight(BlendShape.MouthSmileRight, 0.6f),
        new BlendShapeWeight(BlendShape.JawOpen, 0.2f),
        new BlendShapeWeight(BlendShape.MouthStretchLeft, 0.4f),
        new BlendShapeWeight(BlendShape.MouthStretchRight, 0.4f)
    };

    public static BlendShapeWeight[] CreateViseme_OO() => new[]
    {
        new BlendShapeWeight(BlendShape.MouthPucker, 0.8f),
        new BlendShapeWeight(BlendShape.MouthFunnel, 0.6f),
        new BlendShapeWeight(BlendShape.JawOpen, 0.4f),
        new BlendShapeWeight(BlendShape.MouthRollLower, 0.3f),
        new BlendShapeWeight(BlendShape.MouthRollUpper, 0.3f)
    };

    public static BlendShapeWeight[] CreateViseme_PP() => new[]
    {
        new BlendShapeWeight(BlendShape.MouthClose, 0.9f),
        new BlendShapeWeight(BlendShape.MouthPressLeft, 0.7f),
        new BlendShapeWeight(BlendShape.MouthPressRight, 0.7f),
        new BlendShapeWeight(BlendShape.MouthPucker, 0.2f)
    };

    public static BlendShapeWeight[] CreateViseme_FF() => new[]
    {
        new BlendShapeWeight(BlendShape.MouthLowerDownLeft, 0.3f),
        new BlendShapeWeight(BlendShape.MouthLowerDownRight, 0.3f),
        new BlendShapeWeight(BlendShape.MouthUpperUpLeft, 0.4f),
        new BlendShapeWeight(BlendShape.MouthUpperUpRight, 0.4f),
        new BlendShapeWeight(BlendShape.MouthFunnel, 0.2f)
    };
}
