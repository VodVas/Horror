using UnityEngine;

[CreateAssetMenu(fileName = "EmotionData", menuName = "ScriptableObjects/EmotionSystem/Emotion Data")]
public class EmotionData : ScriptableObject
{
    [SerializeField] private EmotionType emotionType;
    [SerializeField] private BlendShapeWeight[] blendShapes;
    [SerializeField] private float defaultIntensity = 1f;
    [SerializeField] private float defaultDuration = 0.3f;

    public EmotionType Type => emotionType;
    public float DefaultIntensity => defaultIntensity;
    public float DefaultDuration => defaultDuration;
    public BlendShapeWeight[] BlendShapes => blendShapes;

#if UNITY_EDITOR
    public void EditorInitialize(EmotionType type, BlendShapeWeight[] weights)
    {
        if (!Application.isEditor)
            throw new System.InvalidOperationException("Initialization allowed only in Editor mode");

        emotionType = type;
        blendShapes = ValidateWeights(weights);
        defaultIntensity = Mathf.Clamp01(defaultIntensity);
        defaultDuration = Mathf.Max(0, defaultDuration);
    }

    private BlendShapeWeight[] ValidateWeights(BlendShapeWeight[] weights)
    {
        foreach (var weight in weights)
        {
            weight.SetWeight(Mathf.Clamp01(weight.Weight));
        }

        return weights;
    }
#endif
}