#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FacialExpressionController))]
public class FacialExpressionControllerEditor : Editor
{
    private static readonly EmotionType[] BasicEmotions =
    {
        EmotionType.Happy,
        EmotionType.Sad,
        EmotionType.Angry,
        EmotionType.Surprised,
    };

    private static readonly EmotionType[] AdvancedEmotions =
    {
        EmotionType.Disgusted,
        EmotionType.Fearful,
        EmotionType.Confused,
        EmotionType.Hopeful,
        EmotionType.Grateful,
        EmotionType.Jealous,
        EmotionType.Bored,
        EmotionType.EcstaticJoy,
        EmotionType.CosmicHorror,
    };

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        var controller = (FacialExpressionController)target;

        if (!Application.isPlaying) return;

        GUILayout.Space(10);
        GUILayout.Label("Emotion Controls", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        foreach (var emotion in BasicEmotions)
        {
            if (GUILayout.Button(emotion.ToString()))
                controller.SetEmotion(emotion);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        foreach (var emotion in AdvancedEmotions)
        {
            if (GUILayout.Button(emotion.ToString()))
                controller.SetEmotion(emotion);
        }

        if (GUILayout.Button("Neutral"))
            controller.ResetToNeutral();
        EditorGUILayout.EndHorizontal();

        if (controller.IsTransitioning)
        {
            EditorGUILayout.HelpBox("Transition in progress...", MessageType.Info);
        }
    }
}
#endif