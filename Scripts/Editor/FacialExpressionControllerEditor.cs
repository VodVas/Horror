//#if UNITY_EDITOR
//using UnityEditor;
//using UnityEngine;

//[CustomEditor(typeof(FacialExpressionController))]
//public class FacialExpressionControllerEditor : Editor
//{
//    public override void OnInspectorGUI()
//    {
//        DrawDefaultInspector();

//        var controller = (FacialExpressionController)target;
//        if (!Application.isPlaying) return;

//        GUILayout.Space(10);
//        GUILayout.Label("Runtime Controls", EditorStyles.boldLabel);

//        GUILayout.BeginHorizontal();
//        if (GUILayout.Button("Happy")) controller.SetEmotion(EmotionType.Happy);
//        if (GUILayout.Button("Sad")) controller.SetEmotion(EmotionType.Sad);
//        if (GUILayout.Button("Angry")) controller.SetEmotion(EmotionType.Angry);
//        GUILayout.EndHorizontal();

//        GUILayout.BeginHorizontal();
//        if (GUILayout.Button("Surprised")) controller.SetEmotion(EmotionType.Surprised);
//        if (GUILayout.Button("Neutral")) controller.ResetToNeutral();
//        GUILayout.EndHorizontal();

//        if (controller.IsTransitioning)
//        {
//            GUILayout.Label("Transitioning...", EditorStyles.helpBox);
//        }
//    }
//}
//#endif


//#if UNITY_EDITOR
//using UnityEditor;
//using UnityEngine;

//[CustomEditor(typeof(FacialExpressionController))]
//public class FacialExpressionControllerEditor : Editor
//{
//    public override void OnInspectorGUI()
//    {
//        DrawDefaultInspector();

//        var controller = (FacialExpressionController)target;
//        if (!Application.isPlaying) return;

//        GUILayout.Space(10);
//        GUILayout.Label("Runtime Controls", EditorStyles.boldLabel);

//        GUILayout.BeginHorizontal();
//        if (GUILayout.Button("Happy")) controller.SetEmotion(EmotionType.Happy);
//        if (GUILayout.Button("Sad")) controller.SetEmotion(EmotionType.Sad);
//        if (GUILayout.Button("Angry")) controller.SetEmotion(EmotionType.Angry);
//        GUILayout.EndHorizontal();

//        GUILayout.BeginHorizontal();
//        if (GUILayout.Button("Surprised")) controller.SetEmotion(EmotionType.Surprised);
//        if (GUILayout.Button("Neutral")) controller.ResetToNeutral();
//        GUILayout.EndHorizontal();

//        if (controller.IsTransitioning)
//        {
//            GUILayout.Label("Transitioning...", EditorStyles.helpBox);
//        }
//    }
//}
//#endif


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
        EmotionType.Surprised
    };

    private static readonly EmotionType[] AdvancedEmotions =
    {
        EmotionType.Disgusted,
        EmotionType.Fearful,
        EmotionType.Confused
    };

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        var controller = (FacialExpressionController)target;

        if (!Application.isPlaying) return;

        GUILayout.Space(10);
        GUILayout.Label("Emotion Controls", EditorStyles.boldLabel);

        // Basic emotions in one row
        EditorGUILayout.BeginHorizontal();
        foreach (var emotion in BasicEmotions)
        {
            if (GUILayout.Button(emotion.ToString()))
                controller.SetEmotion(emotion);
        }
        EditorGUILayout.EndHorizontal();

        // Advanced emotions in second row
        EditorGUILayout.BeginHorizontal();
        foreach (var emotion in AdvancedEmotions)
        {
            if (GUILayout.Button(emotion.ToString()))
                controller.SetEmotion(emotion);
        }

        if (GUILayout.Button("Neutral"))
            controller.ResetToNeutral();
        EditorGUILayout.EndHorizontal();

        // Transition state indicator
        if (controller.IsTransitioning)
        {
            EditorGUILayout.HelpBox("Transition in progress...", MessageType.Info);
        }
    }
}
#endif