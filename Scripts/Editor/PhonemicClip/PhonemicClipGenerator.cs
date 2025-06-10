using UnityEngine;
using UnityEditor;

public class PhonemicClipGenerator : EditorWindow
{
    private AudioClip _audioClip;
    private string _transcript = "";
    private float _wordsPerMinute = 150f;
    private string _clipName = "NewPhonemicClip";
    private PhonemicClip _previewClip;
    private Vector2 _scrollPosition;

    [MenuItem("Tools/PhonemicAudioSystem/Phonemic Clip Generator")]
    public static void ShowWindow()
    {
        var window = GetWindow<PhonemicClipGenerator>("Phonemic Clip Generator");
        window.minSize = new Vector2(400, 500);
    }

    private void OnGUI()
    {
        DrawHeader();
        DrawInputSection();
        DrawPreviewSection();
        DrawActionButtons();
    }

    private void DrawHeader()
    {
        EditorGUILayout.LabelField("Phonemic Clip Generator", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(
            "Generate lip-sync data from text transcript. " +
            "Supports English phonemes with automatic timing.",
            MessageType.Info);
        EditorGUILayout.Space();
    }

    private void DrawInputSection()
    {
        EditorGUILayout.LabelField("Input Settings", EditorStyles.boldLabel);

        _audioClip = (AudioClip)EditorGUILayout.ObjectField(
            "Audio Clip", _audioClip, typeof(AudioClip), false);

        EditorGUILayout.LabelField("Transcript:");
        _transcript = EditorGUILayout.TextArea(_transcript, GUILayout.Height(100));

        _wordsPerMinute = EditorGUILayout.Slider("Words Per Minute", _wordsPerMinute, 50f, 300f);
        _clipName = EditorGUILayout.TextField("Clip Name", _clipName);

        if (_audioClip != null)
        {
            EditorGUILayout.LabelField($"Duration: {_audioClip.length:F2} seconds");
            int wordCount = CountWords(_transcript);
            float estimatedDuration = (wordCount / _wordsPerMinute) * 60f;
            EditorGUILayout.LabelField($"Estimated Duration: {estimatedDuration:F2} seconds");

            if (Mathf.Abs(estimatedDuration - _audioClip.length) > 2f)
            {
                EditorGUILayout.HelpBox(
                    "Audio duration doesn't match estimated speech duration. " +
                    "Consider adjusting Words Per Minute.",
                    MessageType.Warning);
            }
        }

        EditorGUILayout.Space();
    }

    private void DrawPreviewSection()
    {
        if (_previewClip == null) return;

        EditorGUILayout.LabelField("Preview", EditorStyles.boldLabel);

        _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, GUILayout.Height(150));

        if (_previewClip.Visemes != null)
        {
            EditorGUILayout.LabelField($"Generated {_previewClip.Visemes.Length} visemes:");

            foreach (var viseme in _previewClip.Visemes)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"Time: {viseme.Time:F3}s", GUILayout.Width(100));
                EditorGUILayout.LabelField($"Viseme: {viseme.Viseme}", GUILayout.Width(100));
                EditorGUILayout.LabelField($"Weight: {viseme.Weight:F2}", GUILayout.Width(100));
                EditorGUILayout.EndHorizontal();
            }
        }

        EditorGUILayout.EndScrollView();
        EditorGUILayout.Space();
    }

    private void DrawActionButtons()
    {
        EditorGUILayout.BeginHorizontal();

        GUI.enabled = CanGenerate();
        if (GUILayout.Button("Generate Preview", GUILayout.Height(30)))
        {
            GeneratePreview();
        }

        GUI.enabled = _previewClip != null && CanGenerate();
        if (GUILayout.Button("Save Clip", GUILayout.Height(30)))
        {
            SaveClip();
        }

        GUI.enabled = true;
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Clear", GUILayout.Height(25)))
        {
            ClearAll();
        }
    }

    private bool CanGenerate()
    {
        return _audioClip != null &&
               !string.IsNullOrWhiteSpace(_transcript) &&
               !string.IsNullOrWhiteSpace(_clipName);
    }

    private void GeneratePreview()
    {
        _previewClip = CreateInstance<PhonemicClip>();
        _previewClip.Initialize(_audioClip, _transcript);
        _previewClip.SetWordsPerMinute(_wordsPerMinute);

        var visemes = EnglishPhonemeParser.ParseText(
            _transcript,
            _audioClip.length,
            _wordsPerMinute);

        _previewClip.SetVisemes(visemes.ToArray());

        Debug.Log($"Generated {visemes.Count} visemes for '{_clipName}'");
    }

    private void SaveClip()
    {
        string path = EditorUtility.SaveFilePanelInProject(
            "Save Phonemic Clip",
            _clipName,
            "asset",
            "Choose location for the phonemic clip",
            "Assets/PhonemicClips");

        if (string.IsNullOrEmpty(path)) return;

        AssetDatabase.CreateAsset(_previewClip, path);
        AssetDatabase.SaveAssets();

        EditorUtility.DisplayDialog(
            "Success",
            $"Phonemic clip saved to:\n{path}",
            "OK");

        Selection.activeObject = _previewClip;
        _previewClip = null;
    }

    private void ClearAll()
    {
        _audioClip = null;
        _transcript = "";
        _wordsPerMinute = 150f;
        _clipName = "NewPhonemicClip";
        _previewClip = null;
    }

    private int CountWords(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return 0;
        return text.Split(new[] { ' ', '\n', '\r', '\t' },
            System.StringSplitOptions.RemoveEmptyEntries).Length;
    }
}