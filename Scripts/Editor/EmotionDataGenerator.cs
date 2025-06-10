#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System;
using System.Linq;
using System.Reflection;
using System.IO;

public static class EmotionDataGenerator
{
    private const string BasePath = "Assets/_project/ScriptableObjects/Emotions";
    private const string PresetClassName = nameof(FacialExpressionPresets);

    [MenuItem("Tools/PhonemicAudioSystem/Generate Emotion Presets")]
    public static void GenerateAll()
    {
        try
        {
            var presetType = typeof(FacialExpressionPresets);
            var presets = presetType.GetProperties(BindingFlags.Public | BindingFlags.Static)
                .Where(p => p.PropertyType == typeof(BlendShapeWeight[]))
                .ToArray();

            EditorUtility.DisplayProgressBar("Generating Presets", "Initializing...", 0);

            EnsureDirectoryExists();
            var existingAssets = LoadExistingAssets();

            for (int i = 0; i < presets.Length; i++)
            {
                var preset = presets[i];
                if (!Enum.TryParse<EmotionType>(preset.Name.Replace("Expression", ""), out var emotionType)) continue;

                var progress = (float)i / presets.Length;
                EditorUtility.DisplayProgressBar("Processing Presets", preset.Name, progress);

                var weights = (BlendShapeWeight[])preset.GetValue(null);
                CreateEmotionAsset(emotionType, weights, existingAssets);
            }

            AssetDatabase.SaveAssets();
            EditorUtility.ClearProgressBar();
            EditorUtility.DisplayDialog("Success", $"Generated {presets.Length} emotion presets", "OK");
        }
        catch (Exception e)
        {
            EditorUtility.ClearProgressBar();
            EditorUtility.DisplayDialog("Error", $"Generation failed: {e.Message}", "OK");
            Debug.LogException(e);
        }
    }

    private static void EnsureDirectoryExists()
    {
        if (!Directory.Exists(BasePath))
        {
            Directory.CreateDirectory(BasePath);
            AssetDatabase.Refresh();
        }
    }

    private static EmotionData[] LoadExistingAssets()
    {
        var guids = AssetDatabase.FindAssets($"t:{nameof(EmotionData)}", new[] { BasePath });
        return guids.Select(guid =>
            AssetDatabase.LoadAssetAtPath<EmotionData>(
                AssetDatabase.GUIDToAssetPath(guid))).ToArray();
    }

    private static void CreateEmotionAsset(EmotionType type, BlendShapeWeight[] weights, EmotionData[] existing)
    {
        var assetName = $"{type}Emotion.asset";
        var existingAsset = existing.FirstOrDefault(e => e.Type == type);

        if (existingAsset != null)
        {
            if (!EditorUtility.DisplayDialog("Asset Exists",
                $"Emotion {type} already exists. Overwrite?", "Yes", "No")) return;

            existingAsset.SafeInitialize(type, weights);
        }
        else
        {
            var data = ScriptableObject.CreateInstance<EmotionData>();
            data.SafeInitialize(type, weights);
            AssetDatabase.CreateAsset(data, $"{BasePath}/{assetName}");
        }
    }
}

public static class EmotionDataExtensions
{
    public static void SafeInitialize(this EmotionData data, EmotionType type, BlendShapeWeight[] weights)
    {
        if (data == null) throw new System.ArgumentNullException(nameof(data));
        if (!Application.isEditor) return;

        try
        {
            data.EditorInitialize(type, weights);
            EditorUtility.SetDirty(data);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to initialize EmotionData: {e.Message}");
            throw;
        }
    }
}
#endif