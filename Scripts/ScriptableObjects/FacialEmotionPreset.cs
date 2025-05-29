using System.Collections.Generic;
using System.Linq;
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

    public EmotionData GetEmotion(EmotionType type) => (int)type < emotions.Length ? emotions[(int)type] : null;

    private List<EmotionData> _cachedNonNeutralEmotions;
    private bool _isCacheValid;

    public EmotionData GetRandomNonNeutralEmotion()
    {
        if (!_isCacheValid)
        {
            _cachedNonNeutralEmotions = emotions
                .Where(e => e != null && e.Type != EmotionType.Neutral)
                .ToList();
            _isCacheValid = true;
        }

        if (_cachedNonNeutralEmotions.Count == 0) return null;
        return _cachedNonNeutralEmotions[Random.Range(0, _cachedNonNeutralEmotions.Count)];
    }

#if UNITY_EDITOR
    private void OnValidate() => _isCacheValid = false;
#endif
}