using UnityEngine;

[CreateAssetMenu(fileName = "FacialEmotionPreset", menuName = "ScriptableObjects/EmotionSystem/Emotion Preset")]
public class FacialEmotionPreset : ScriptableObject
{
    [SerializeField]
    private EmotionData[] emotions = new EmotionData[]
    {
        null, // Neutral
        null, // Happy  
        null, // Sad
        null, // Angry
        null, // Surprised
        null, // Disgusted
        null, // Fearful
        null  // Confused
    };

    public EmotionData GetEmotion(EmotionType type) =>
        (int)type < emotions.Length ? emotions[(int)type] : null;
}
