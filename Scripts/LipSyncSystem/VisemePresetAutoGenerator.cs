#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

public static class VisemePresetAutoGenerator
{
    private struct VisemeDefinition
    {
        public VisemeType Type;
        public Dictionary<BlendShape, float> Weights;
        public float TransitionSpeed;

        public VisemeDefinition(VisemeType type, float speed = 10f)
        {
            Type = type;
            Weights = new Dictionary<BlendShape, float>();
            TransitionSpeed = speed;
        }
    }

    [MenuItem("Tools/PhonemicAudioSystem/Generate Viseme Preset")]
    public static void GenerateVisemePreset()
    {
        try
        {
            EditorUtility.DisplayProgressBar("Generating Viseme Preset", "Creating mappings...", 0f);

            var preset = ScriptableObject.CreateInstance<VisemePreset>();
            var mappings = GenerateScientificVisemeMappings();

            var mappingArray = new VisemeMapping[mappings.Count];
            int index = 0;

            foreach (var mapping in mappings)
            {
                EditorUtility.DisplayProgressBar("Generating Viseme Preset",
                    $"Processing {mapping.Type}...", (float)index / mappings.Count);

                var visemeMapping = new VisemeMapping
                {
                    Viseme = mapping.Type,
                    Weights = mapping.Weights.Select(kv =>
                        new BlendShapeWeight(kv.Key, kv.Value)).ToArray(),
                    TransitionSpeed = mapping.TransitionSpeed
                };

                mappingArray[index++] = visemeMapping;
            }

            var serializedPreset = new SerializedObject(preset);
            serializedPreset.FindProperty("visemeMappings").arraySize = mappingArray.Length;

            for (int i = 0; i < mappingArray.Length; i++)
            {
                var element = serializedPreset.FindProperty("visemeMappings").GetArrayElementAtIndex(i);
                element.FindPropertyRelative("Viseme").enumValueIndex = (int)mappingArray[i].Viseme;
                element.FindPropertyRelative("TransitionSpeed").floatValue = mappingArray[i].TransitionSpeed;

                var weightsProperty = element.FindPropertyRelative("Weights");
                weightsProperty.arraySize = mappingArray[i].Weights.Length;

                for (int j = 0; j < mappingArray[i].Weights.Length; j++)
                {
                    var weight = mappingArray[i].Weights[j];
                    var weightElement = weightsProperty.GetArrayElementAtIndex(j);
                    weightElement.FindPropertyRelative("<BlendShape>k__BackingField").enumValueIndex = (int)weight.BlendShape;
                    weightElement.FindPropertyRelative("<Weight>k__BackingField").floatValue = weight.Weight;
                }
            }

            serializedPreset.ApplyModifiedProperties();

            string path = EditorUtility.SaveFilePanelInProject(
                "Save Viseme Preset",
                "GeneratedVisemePreset",
                "asset",
                "Choose location for the viseme preset");

            if (!string.IsNullOrEmpty(path))
            {
                AssetDatabase.CreateAsset(preset, path);
                AssetDatabase.SaveAssets();
                EditorUtility.DisplayDialog("Success",
                    $"Viseme preset generated successfully!\nSaved to: {path}", "OK");
                Selection.activeObject = preset;
            }

            EditorUtility.ClearProgressBar();
        }
        catch (System.Exception e)
        {
            EditorUtility.ClearProgressBar();
            EditorUtility.DisplayDialog("Error", $"Failed to generate preset: {e.Message}", "OK");
            Debug.LogException(e);
        }
    }

    private static List<VisemeDefinition> GenerateScientificVisemeMappings()
    {
        var mappings = new List<VisemeDefinition>();

        var silent = new VisemeDefinition(VisemeType.Silent, 15f);
        mappings.Add(silent);

        var pp = new VisemeDefinition(VisemeType.PP, 12f);
        pp.Weights[BlendShape.MouthClose] = 0.95f;
        pp.Weights[BlendShape.MouthPressLeft] = 0.8f;
        pp.Weights[BlendShape.MouthPressRight] = 0.8f;
        pp.Weights[BlendShape.MouthPucker] = 0.2f;
        pp.Weights[BlendShape.JawOpen] = 0.05f;
        mappings.Add(pp);

        var ff = new VisemeDefinition(VisemeType.FF, 10f);
        ff.Weights[BlendShape.MouthLowerDownLeft] = 0.4f;
        ff.Weights[BlendShape.MouthLowerDownRight] = 0.4f;
        ff.Weights[BlendShape.MouthUpperUpLeft] = 0.5f;
        ff.Weights[BlendShape.MouthUpperUpRight] = 0.5f;
        ff.Weights[BlendShape.MouthFunnel] = 0.3f;
        ff.Weights[BlendShape.JawOpen] = 0.15f;
        mappings.Add(ff);

        var th = new VisemeDefinition(VisemeType.TH, 10f);
        th.Weights[BlendShape.JawOpen] = 0.2f;
        th.Weights[BlendShape.JawOpen] = 0.3f;
        th.Weights[BlendShape.MouthStretchLeft] = 0.2f;
        th.Weights[BlendShape.MouthStretchRight] = 0.2f;
        mappings.Add(th);

        var dd = new VisemeDefinition(VisemeType.DD, 12f);
        dd.Weights[BlendShape.JawOpen] = 0.25f;
        th.Weights[BlendShape.JawOpen] = 0.3f;
        dd.Weights[BlendShape.MouthDimpleLeft] = 0.15f;
        dd.Weights[BlendShape.MouthDimpleRight] = 0.15f;
        mappings.Add(dd);

        var kk = new VisemeDefinition(VisemeType.KK, 11f);
        kk.Weights[BlendShape.JawOpen] = 0.35f;
        kk.Weights[BlendShape.MouthFunnel] = 0.2f;
        kk.Weights[BlendShape.MouthLowerDownLeft] = 0.2f;
        kk.Weights[BlendShape.MouthLowerDownRight] = 0.2f;
        mappings.Add(kk);

        var ch = new VisemeDefinition(VisemeType.CH, 10f);
        ch.Weights[BlendShape.MouthPucker] = 0.6f;
        ch.Weights[BlendShape.MouthFunnel] = 0.5f;
        ch.Weights[BlendShape.JawOpen] = 0.2f;
        ch.Weights[BlendShape.MouthUpperUpLeft] = 0.3f;
        ch.Weights[BlendShape.MouthUpperUpRight] = 0.3f;
        mappings.Add(ch);

        var ss = new VisemeDefinition(VisemeType.SS, 10f);
        ss.Weights[BlendShape.MouthSmileLeft] = 0.3f;
        ss.Weights[BlendShape.MouthSmileRight] = 0.3f;
        ss.Weights[BlendShape.JawOpen] = 0.1f;
        ss.Weights[BlendShape.MouthDimpleLeft] = 0.2f;
        ss.Weights[BlendShape.MouthDimpleRight] = 0.2f;
        mappings.Add(ss);

        var nn = new VisemeDefinition(VisemeType.NN, 11f);
        nn.Weights[BlendShape.JawOpen] = 0.15f;
        th.Weights[BlendShape.JawOpen] = 0.3f;
        nn.Weights[BlendShape.NoseSneerLeft] = 0.1f;
        nn.Weights[BlendShape.NoseSneerRight] = 0.1f;
        mappings.Add(nn);

        var rr = new VisemeDefinition(VisemeType.RR, 10f);
        rr.Weights[BlendShape.MouthPucker] = 0.4f;
        rr.Weights[BlendShape.MouthFunnel] = 0.3f;
        rr.Weights[BlendShape.JawOpen] = 0.2f;
        rr.Weights[BlendShape.MouthRollUpper] = 0.2f;
        rr.Weights[BlendShape.MouthRollLower] = 0.2f;
        mappings.Add(rr);

        var aa = new VisemeDefinition(VisemeType.AA, 8f);
        aa.Weights[BlendShape.JawOpen] = 0.7f;
        th.Weights[BlendShape.JawOpen] = 0.3f;
        aa.Weights[BlendShape.MouthLowerDownLeft] = 0.5f;
        aa.Weights[BlendShape.MouthLowerDownRight] = 0.5f;
        aa.Weights[BlendShape.MouthStretchLeft] = 0.3f;
        aa.Weights[BlendShape.MouthStretchRight] = 0.3f;
        mappings.Add(aa);

        var ee = new VisemeDefinition(VisemeType.EE, 8f);
        ee.Weights[BlendShape.MouthSmileLeft] = 0.6f;
        ee.Weights[BlendShape.MouthSmileRight] = 0.6f;
        ee.Weights[BlendShape.JawOpen] = 0.25f;
        ee.Weights[BlendShape.MouthStretchLeft] = 0.4f;
        ee.Weights[BlendShape.MouthStretchRight] = 0.4f;
        ee.Weights[BlendShape.MouthDimpleLeft] = 0.2f;
        ee.Weights[BlendShape.MouthDimpleRight] = 0.2f;
        mappings.Add(ee);

        var ii = new VisemeDefinition(VisemeType.II, 8f);
        ii.Weights[BlendShape.MouthSmileLeft] = 0.4f;
        ii.Weights[BlendShape.MouthSmileRight] = 0.4f;
        ii.Weights[BlendShape.JawOpen] = 0.15f;
        ii.Weights[BlendShape.MouthStretchLeft] = 0.5f;
        ii.Weights[BlendShape.MouthStretchRight] = 0.5f;
        ii.Weights[BlendShape.CheekSquintLeft] = 0.1f;
        ii.Weights[BlendShape.CheekSquintRight] = 0.1f;
        mappings.Add(ii);

        var oo = new VisemeDefinition(VisemeType.OO, 8f);
        oo.Weights[BlendShape.MouthPucker] = 0.8f;
        oo.Weights[BlendShape.MouthFunnel] = 0.7f;
        oo.Weights[BlendShape.JawOpen] = 0.45f;
        oo.Weights[BlendShape.MouthRollLower] = 0.3f;
        oo.Weights[BlendShape.MouthRollUpper] = 0.3f;
        oo.Weights[BlendShape.MouthShrugLower] = 0.2f;
        oo.Weights[BlendShape.MouthShrugUpper] = 0.2f;
        mappings.Add(oo);

        var uu = new VisemeDefinition(VisemeType.UU, 8f);
        uu.Weights[BlendShape.MouthPucker] = 0.9f;
        uu.Weights[BlendShape.MouthFunnel] = 0.8f;
        uu.Weights[BlendShape.JawOpen] = 0.2f;
        uu.Weights[BlendShape.MouthClose] = 0.3f;
        uu.Weights[BlendShape.MouthShrugLower] = 0.3f;
        uu.Weights[BlendShape.MouthShrugUpper] = 0.3f;
        mappings.Add(uu);

        return OptimizeVisemeWeights(mappings);
    }

    private static List<VisemeDefinition> OptimizeVisemeWeights(List<VisemeDefinition> mappings)
    {
        foreach (var mapping in mappings)
        {
            float totalWeight = mapping.Weights.Values.Sum();
            if (totalWeight > 2.5f)
            {
                float scale = 2.5f / totalWeight;
                var keys = mapping.Weights.Keys.ToList();
                foreach (var key in keys)
                {
                    mapping.Weights[key] *= scale;
                }
            }

            var keysToRemove = mapping.Weights
                .Where(kv => kv.Value < 0.05f)
                .Select(kv => kv.Key)
                .ToList();

            foreach (var key in keysToRemove)
            {
                mapping.Weights.Remove(key);
            }
        }

        return mappings;
    }
}
#endif