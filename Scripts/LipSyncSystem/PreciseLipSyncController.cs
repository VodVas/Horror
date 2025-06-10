using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PreciseLipSyncController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private FacialExpressionController _facialController;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private VisemePreset _visemePreset;

    [Header("Settings")]
    [SerializeField] private float _blendSpeed = 15f;
    [SerializeField] private float _visemeIntensity = 1f;
    [SerializeField] private float _anticipationTime = 0.05f;

    [Header("Clips")]
    [SerializeField] private PhonemicClip[] _clips;

    private Dictionary<string, PhonemicClip> _clipDatabase;
    private Queue<TimedViseme> _upcomingVisemes;
    private VisemeType _currentViseme;
    private VisemeType _targetViseme;
    private float _visemeBlend;
    private float _playbackTime;
    private bool _isPlaying;

    private readonly Dictionary<VisemeType, float[]> _visemeWeightCache = new Dictionary<VisemeType, float[]>();
    private readonly float[] _currentWeights = new float[52];
    private readonly float[] _targetWeights = new float[52];

    public bool IsPlaying => _isPlaying;

    private void Awake()
    {
        InitializeClipDatabase();
        InitializeVisemeCache();
        _visemePreset.Initialize();
    }

    private void Update()
    {
        if (!_isPlaying) return;

        _playbackTime = _audioSource.time;
        ProcessVisemeQueue();
        UpdateVisemeBlend();
        ApplyVisemeWeights();
    }

    public void ResetToNeutral()
    {
        _facialController?.ResetToNeutral(0.3f);
    }


    [ContextMenu("Play")]
    public void Play()
    {
        PlayDialogue(_clips[0].name);
    }

    public void StopDialogue()
    {
        _isPlaying = false;
        _audioSource.Stop();
        _upcomingVisemes.Clear();
    }

    public void PlayDialogue(string clipName)
    {
        if (!_clipDatabase.TryGetValue(clipName, out var clip)) return;

        StopDialogue();

        _audioSource.clip = clip.Clip;
        _audioSource.Play();

        LoadVisemeQueue(clip);
        _isPlaying = true;
        _playbackTime = 0f;
    }

    private void InitializeClipDatabase()
    {
        _clipDatabase = _clips.ToDictionary(c => c.Clip.name);
        _upcomingVisemes = new Queue<TimedViseme>();
    }

    private void InitializeVisemeCache()
    {
        foreach (VisemeType viseme in System.Enum.GetValues(typeof(VisemeType)))
        {
            _visemeWeightCache[viseme] = new float[52];
        }
    }

    private void LoadVisemeQueue(PhonemicClip clip)
    {
        _upcomingVisemes.Clear();
        foreach (var viseme in clip.Visemes.OrderBy(v => v.Time))
        {
            _upcomingVisemes.Enqueue(viseme);
        }
    }

    private void ProcessVisemeQueue()
    {
        while (_upcomingVisemes.Count > 0)
        {
            var nextViseme = _upcomingVisemes.Peek();
            if (_playbackTime + _anticipationTime >= nextViseme.Time)
            {
                SetTargetViseme(nextViseme.Viseme, nextViseme.Weight);
                _upcomingVisemes.Dequeue();
            }
            else break;
        }

        if (!_audioSource.isPlaying)
        {
            StopDialogue();
        }
    }

    private void SetTargetViseme(VisemeType viseme, float weight)
    {
        _targetViseme = viseme;
        _visemeBlend = 0f;

        var mapping = _visemePreset.GetViseme(viseme);
        if (mapping == null) return;

        System.Array.Clear(_targetWeights, 0, _targetWeights.Length);

        foreach (var blendWeight in mapping.Weights)
        {
            int index = (int)blendWeight.BlendShape;
            _targetWeights[index] = blendWeight.Weight * weight * _visemeIntensity;
        }
    }

    private void UpdateVisemeBlend()
    {
        _visemeBlend = Mathf.MoveTowards(_visemeBlend, 1f, _blendSpeed * Time.deltaTime);

        if (_visemeBlend >= 0.99f)
        {
            _currentViseme = _targetViseme;
            System.Array.Copy(_targetWeights, _currentWeights, _targetWeights.Length);
        }
    }

    private void ApplyVisemeWeights()
    {
        var blendedWeights = new BlendShapeWeight[52];

        for (int i = 0; i < 52; i++)
        {
            float weight = Mathf.Lerp(_currentWeights[i], _targetWeights[i], _visemeBlend);
            if (weight > 0.01f)
            {
                blendedWeights[i] = new BlendShapeWeight((BlendShape)i, weight);
            }
        }

        _facialController.SetCustomExpression(blendedWeights.Where(w => w != null).ToArray(), 0.05f);
    }
}