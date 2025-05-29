using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class FacialExpressionController : MonoBehaviour, IFacialExpressionController
{
    const int BlendShapeCount = (int)BlendShape.NoseSneerRight + 1;

    [SerializeField] private SkinnedMeshRenderer faceMesh;
    [SerializeField] private EmotionData[] emotionDatabase;
    [SerializeField] private bool autoInitialize = true;

    private readonly Dictionary<BlendShape, int> _blendShapeIndices = new Dictionary<BlendShape, int>(BlendShapeCount);
    private readonly Dictionary<EmotionType, EmotionData> _emotions = new Dictionary<EmotionType, EmotionData>();
    private readonly float[] _currentWeights = new float[BlendShapeCount];
    private readonly float[] _targetWeights = new float[BlendShapeCount];
    private readonly float[] _startWeights = new float[BlendShapeCount];

    private float _transitionTime;
    private float _transitionDuration;
    private bool _isTransitioning;
    private EmotionType _currentEmotion = EmotionType.Neutral;

    public bool IsTransitioning => _isTransitioning;
    public event Action<EmotionType> OnEmotionChanged;

    private static readonly string[] BlendShapeNames = {
        "blendShape1.browDownLeft", "blendShape1.browDownRight", "blendShape1.browInnerUp",
        "blendShape1.browOuterUpLeft", "blendShape1.browOuterUpRight", "blendShape1.cheekPuff",
        "blendShape1.cheekSquintLeft", "blendShape1.cheekSquintRight", "blendShape1.eyeBlinkLeft",
        "blendShape1.eyeBlinkRight", "blendShape1.eyeLookDownLeft", "blendShape1.eyeLookDownRight",
        "blendShape1.eyeLookInLeft", "blendShape1.eyeLookInRight", "blendShape1.eyeLookOutLeft",
        "blendShape1.eyeLookOutRight", "blendShape1.eyeLookUpLeft", "blendShape1.eyeLookUpRight",
        "blendShape1.eyeSquintLeft", "blendShape1.eyeSquintRight", "blendShape1.eyeWideLeft",
        "blendShape1.eyeWideRight", "blendShape1.jawForward", "blendShape1.jawLeft",
        "blendShape1.jawOpen", "blendShape1.jawRight", "blendShape1.mouthClose",
        "blendShape1.mouthDimpleLeft", "blendShape1.mouthDimpleRight", "blendShape1.mouthFrownLeft",
        "blendShape1.mouthFrownRight", "blendShape1.mouthFunnel", "blendShape1.mouthLeft",
        "blendShape1.mouthLowerDownLeft", "blendShape1.mouthLowerDownRight", "blendShape1.mouthPressLeft",
        "blendShape1.mouthPressRight", "blendShape1.mouthPucke", "blendShape1.mouthRight",
        "blendShape1.mouthRollLower", "blendShape1.mouthRollUpper", "blendShape1.mouthShrugLower",
        "blendShape1.mouthShrugUpper", "blendShape1.mouthSmileLeft", "blendShape1.mouthSmileRight",
        "blendShape1.mouthStretchLeft", "blendShape1.mouthStretchRight", "blendShape1.mouthUpperUpLeft",
        "blendShape1.mouthUpperUpRight", "blendShape1.noseSneerLeft", "blendShape1.noseSneerRight"
    };

    private void Awake()
    {
        if (autoInitialize) Initialize();
    }

    private void Update()
    {
        if (_isTransitioning) UpdateTransition();
    }

    public void Initialize()
    {
        if (!faceMesh) faceMesh = GetComponent<SkinnedMeshRenderer>();
        if (!faceMesh) throw new Exception("SkinnedMeshRenderer not found");

        CacheBlendShapeIndices();
        BuildEmotionDatabase();
        ResetToNeutral(0f);
    }

    private void CacheBlendShapeIndices()
    {
        _blendShapeIndices.Clear();
        var mesh = faceMesh.sharedMesh;

        for (int i = 0; i < BlendShapeNames.Length; i++)
        {
            int index = mesh.GetBlendShapeIndex(BlendShapeNames[i]);
            if (index >= 0)
                _blendShapeIndices[(BlendShape)i] = index;
        }
    }

    private void BuildEmotionDatabase()
    {
        _emotions.Clear();
        foreach (var emotion in emotionDatabase)
            if (emotion) _emotions[emotion.Type] = emotion;
    }

    public void SetEmotion(EmotionType emotion)
    {
        if (!_emotions.TryGetValue(emotion, out var emotionData)) return;

        SetEmotion(emotion, emotionData.DefaultIntensity, emotionData.DefaultDuration);
    }

    public void SetEmotion(EmotionType emotion, float intensity = 1f, float duration = 0.3f)
    {
        if (!_emotions.TryGetValue(emotion, out var emotionData)) return;

        Array.Clear(_targetWeights, 0, BlendShapeCount);

        foreach (var weight in emotionData.BlendShapes)
        {
            int index = (int)weight.BlendShape;
            _targetWeights[index] = Mathf.Clamp01(weight.Weight * intensity);
        }

        StartTransition(duration);

        if (_currentEmotion != emotion)
        {
            _currentEmotion = emotion;
            OnEmotionChanged?.Invoke(emotion);
        }
    }

    public void SetBlendShape(BlendShape blendShape, float weight, float duration = 0f)
    {
        int index = (int)blendShape;
        _targetWeights[index] = Mathf.Clamp01(weight);

        if (duration > 0f)
            StartTransition(duration);
        else
            ApplyBlendShapeWeight(blendShape, weight);
    }

    public void SetCustomExpression(BlendShapeWeight[] weights, float duration = 0.3f)
    {
        Array.Clear(_targetWeights, 0, BlendShapeCount);

        foreach (var weight in weights)
        {
            int index = (int)weight.BlendShape;
            _targetWeights[index] = Mathf.Clamp01(weight.Weight);
        }

        StartTransition(duration);
        _currentEmotion = EmotionType.Neutral;
    }

    public void BlendEmotions(EmotionType primary, EmotionType secondary, float blend, float duration = 0.3f)
    {
        if (!_emotions.TryGetValue(primary, out var primaryData) ||
            !_emotions.TryGetValue(secondary, out var secondaryData)) return;

        Array.Clear(_targetWeights, 0, BlendShapeCount);

        foreach (var weight in primaryData.BlendShapes)
        {
            int index = (int)weight.BlendShape;
            _targetWeights[index] = weight.Weight * (1f - blend);
        }

        foreach (var weight in secondaryData.BlendShapes)
        {
            int index = (int)weight.BlendShape;
            _targetWeights[index] += weight.Weight * blend;
            _targetWeights[index] = Mathf.Clamp01(_targetWeights[index]);
        }

        StartTransition(duration);
        _currentEmotion = blend > 0.5f ? secondary : primary;
    }

    public void ResetToNeutral(float duration = 0.3f)
    {
        Array.Clear(_targetWeights, 0, BlendShapeCount);

        if (duration > 0f)
            StartTransition(duration);
        else
            ApplyAllWeights();

        _currentEmotion = EmotionType.Neutral;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float GetBlendShapeWeight(BlendShape blendShape)
    {
        return _currentWeights[(int)blendShape];
    }

    private void StartTransition(float duration)
    {
        if (duration <= 0f)
        {
            ApplyAllWeights();
            return;
        }

        Array.Copy(_currentWeights, _startWeights, BlendShapeCount);
        _transitionTime = 0f;
        _transitionDuration = duration;
        _isTransitioning = true;
    }

    private void UpdateTransition()
    {
        _transitionTime += Time.deltaTime;
        float t = _transitionTime / _transitionDuration;

        if (t >= 1f)
        {
            t = 1f;
            _isTransitioning = false;
        }

        t = Mathf.SmoothStep(0f, 1f, t);

        for (int i = 0; i < BlendShapeCount; i++)
        {
            float weight = Mathf.Lerp(_startWeights[i], _targetWeights[i], t);
            SetBlendShapeWeightDirect((BlendShape)i, weight);
        }
    }

    private void ApplyAllWeights()
    {
        for (int i = 0; i < BlendShapeCount; i++)
        {
            SetBlendShapeWeightDirect((BlendShape)i, _targetWeights[i]);
        }
        _isTransitioning = false;
    }

    private void ApplyBlendShapeWeight(BlendShape blendShape, float weight)
    {
        SetBlendShapeWeightDirect(blendShape, weight);
        _currentWeights[(int)blendShape] = weight;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void SetBlendShapeWeightDirect(BlendShape blendShape, float weight)
    {
        if (_blendShapeIndices.TryGetValue(blendShape, out int index))
        {
            faceMesh.SetBlendShapeWeight(index, weight * 100f);
            _currentWeights[(int)blendShape] = weight;
        }
    }
}