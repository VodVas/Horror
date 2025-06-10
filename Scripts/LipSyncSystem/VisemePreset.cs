using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "VisemePreset", menuName = "ScriptableObjects/EmotionSystem/Viseme Preset")]
public class VisemePreset : ScriptableObject
{
    [SerializeField] private VisemeMapping[] visemeMappings;
    private Dictionary<VisemeType, VisemeMapping> _mappingCache;

    public void Initialize()
    {
        _mappingCache = visemeMappings.ToDictionary(v => v.Viseme);
    }

    public VisemeMapping GetViseme(VisemeType type)
    {
        return _mappingCache.TryGetValue(type, out var mapping) ? mapping : null;
    }
}