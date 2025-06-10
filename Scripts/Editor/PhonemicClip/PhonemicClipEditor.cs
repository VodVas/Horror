#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

[CustomEditor(typeof(PhonemicClip))]
public class PhonemicClipEditor : Editor
{
    private float _currentTime;
    private VisemeType _selectedViseme = VisemeType.AA;
    private bool _showTimeline = true;
    private bool _showVisemeList = true;

    private Vector2 _visemeListScrollPosition;
    private SerializedProperty _visemesProperty;

    private void OnEnable()
    {
        _visemesProperty = serializedObject.FindProperty("_visemes");
    }

    public override void OnInspectorGUI()
    {
        var clip = (PhonemicClip)target;

        serializedObject.Update();

        DrawHeader(clip);

        if (_showTimeline)
            DrawTimeline(clip);

        if (_showVisemeList)
            DrawVisemeList(clip);

        DrawControls(clip);

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawHeader(PhonemicClip clip)
    {
        EditorGUILayout.LabelField("Phonemic Clip", EditorStyles.boldLabel);

        GUI.enabled = false;
        EditorGUILayout.ObjectField("Audio Clip", clip.Clip, typeof(AudioClip), false);
        EditorGUILayout.TextField("Transcript", clip.Transcript);
        GUI.enabled = true;

        EditorGUILayout.Space();

        _showTimeline = EditorGUILayout.Foldout(_showTimeline, "Timeline View");
        _showVisemeList = EditorGUILayout.Foldout(_showVisemeList, "Viseme List");
    }

    private void DrawTimeline(PhonemicClip clip)
    {
        if (clip.Clip == null) return;

        var rect = GUILayoutUtility.GetRect(300, 60);
        GUI.Box(rect, GUIContent.none);

        if (clip.Visemes == null) return;

        for (int i = 0; i <= 5; i++)
        {
            float time = clip.Clip.length * i / 5f;
            float x = rect.x + (time / clip.Clip.length) * rect.width;

            EditorGUI.DrawRect(new Rect(x - 0.5f, rect.y, 1, rect.height), Color.gray);
            GUI.Label(new Rect(x - 15, rect.y + rect.height, 30, 20), $"{time:F1}s");
        }

        foreach (var viseme in clip.Visemes)
        {
            float x = rect.x + (viseme.Time / clip.Clip.length) * rect.width;
            float height = rect.height * viseme.Weight * 0.8f;

            var visemeRect = new Rect(x - 2, rect.y + rect.height - height, 4, height);
            EditorGUI.DrawRect(visemeRect, GetVisemeColor(viseme.Viseme));

            if (visemeRect.Contains(Event.current.mousePosition))
            {
                GUI.Label(new Rect(x - 30, rect.y - 20, 60, 20),
                    $"{viseme.Viseme} @ {viseme.Time:F2}s");
            }
        }

        if (_currentTime >= 0 && _currentTime <= clip.Clip.length)
        {
            float x = rect.x + (_currentTime / clip.Clip.length) * rect.width;
            EditorGUI.DrawRect(new Rect(x - 1, rect.y, 2, rect.height), Color.red);
        }

        EditorGUILayout.Space();
    }

    private void DrawVisemeList(PhonemicClip clip)
    {
        if (_visemesProperty == null || _visemesProperty.arraySize == 0)
        {
            EditorGUILayout.HelpBox("No visemes generated", MessageType.Info);
            return;
        }

        EditorGUILayout.LabelField($"Total Visemes: {_visemesProperty.arraySize}");

        float scrollHeight = Mathf.Min(200f, _visemesProperty.arraySize * 22f);

        _visemeListScrollPosition = EditorGUILayout.BeginScrollView(
            _visemeListScrollPosition,
            GUILayout.Height(scrollHeight));

        EditorGUI.BeginChangeCheck();

        for (int i = 0; i < _visemesProperty.arraySize; i++)
        {
            var element = _visemesProperty.GetArrayElementAtIndex(i);

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField($"{i}:", GUILayout.Width(30));

            var timeProperty = element.FindPropertyRelative("Time");
            EditorGUILayout.LabelField($"{timeProperty.floatValue:F3}s", GUILayout.Width(60));

            var visemeProperty = element.FindPropertyRelative("Viseme");
            EditorGUILayout.LabelField(((VisemeType)visemeProperty.enumValueIndex).ToString(), GUILayout.Width(80));

            var weightProperty = element.FindPropertyRelative("Weight");
            weightProperty.floatValue = EditorGUILayout.Slider(weightProperty.floatValue, 0f, 1f);

            if (GUILayout.Button("X", GUILayout.Width(20)))
            {
                _visemesProperty.DeleteArrayElementAtIndex(i);
                break;
            }

            EditorGUILayout.EndHorizontal();
        }

        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
        }

        EditorGUILayout.EndScrollView();
        EditorGUILayout.Space();
    }

    private void DrawControls(PhonemicClip clip)
    {
        if (clip.Clip == null) return;

        EditorGUILayout.LabelField("Manual Edit", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        _currentTime = EditorGUILayout.Slider("Time", _currentTime, 0, clip.Clip.length);
        _selectedViseme = (VisemeType)EditorGUILayout.EnumPopup(_selectedViseme);

        if (GUILayout.Button("Add", GUILayout.Width(50)))
        {
            AddViseme(clip, _currentTime, _selectedViseme);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        if (GUILayout.Button("Regenerate from Transcript"))
        {
            RegenerateVisemes(clip);
        }

        if (GUILayout.Button("Clear All Visemes"))
        {
            if (EditorUtility.DisplayDialog("Clear Visemes",
                "Are you sure you want to clear all visemes?", "Yes", "No"))
            {
                clip.SetVisemes(new TimedViseme[0]);
                serializedObject.Update();
            }
        }
    }

    private void AddViseme(PhonemicClip clip, float time, VisemeType viseme)
    {
        var list = clip.Visemes?.ToList() ?? new List<TimedViseme>();
        list.Add(new TimedViseme
        {
            Time = time,
            Viseme = viseme,
            Weight = EnglishPhonemeParser.GetVisemeWeight(viseme)
        });
        clip.SetVisemes(list.OrderBy(v => v.Time).ToArray());
        serializedObject.Update();
    }

    private void RegenerateVisemes(PhonemicClip clip)
    {
        if (string.IsNullOrWhiteSpace(clip.Transcript))
        {
            EditorUtility.DisplayDialog("Error",
                "Transcript is empty. Cannot generate visemes.", "OK");
            return;
        }

        var visemes = EnglishPhonemeParser.ParseText(
            clip.Transcript,
            clip.Clip.length,
            clip.WordsPerMinute);

        clip.SetVisemes(visemes.ToArray());
        serializedObject.Update();
        Debug.Log($"Regenerated {visemes.Count} visemes");
    }

    private Color GetVisemeColor(VisemeType viseme)
    {
        return viseme switch
        {
            VisemeType.Silent => Color.gray,
            VisemeType.PP => Color.red,
            VisemeType.FF => new Color(1f, 0.5f, 0f),
            VisemeType.TH => Color.yellow,
            VisemeType.DD => Color.green,
            VisemeType.KK => Color.cyan,
            VisemeType.CH => Color.blue,
            VisemeType.SS => new Color(0.5f, 0f, 1f),
            VisemeType.NN => Color.magenta,
            VisemeType.RR => new Color(1f, 0f, 0.5f),
            VisemeType.AA => new Color(1f, 0.8f, 0.8f),
            VisemeType.EE => new Color(0.8f, 1f, 0.8f),
            VisemeType.II => new Color(0.8f, 0.8f, 1f),
            VisemeType.OO => new Color(1f, 1f, 0.8f),
            VisemeType.UU => new Color(1f, 0.8f, 1f),
            _ => Color.white
        };
    }
}
#endif