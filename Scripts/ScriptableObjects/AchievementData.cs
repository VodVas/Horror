using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Achievements/Achievement Data")]
public class AchievementData : ScriptableObject
{
    [field: SerializeField] public string DisplayName { get; private set; }
    [field: SerializeField, TextArea] public string Description { get; private set; }
    [field: SerializeField] public int RequiredHits { get; private set; } = 5;
    [field: SerializeField] public float DisplayDuration { get; private set; } = 3f;
    [field: SerializeField] public float FadeDuration { get; private set; } = 0.5f;
}