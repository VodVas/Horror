using UnityEngine;

public class SimpleLipSyncController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private FacialExpressionController facialController;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip audioClip;

    [Header("Settings")]
    [SerializeField] private float sensitivity = 100f;
    [SerializeField] private float smoothSpeed = 10f;
    [SerializeField] private AnimationCurve responseCurve = AnimationCurve.Linear(0, 0, 1, 1);
    [SerializeField] private float minThreshold = 0.01f;

    [Header("Viseme Weights")]
    [SerializeField]
    private BlendShapeWeight[] openMouthWeights = new[]
    {
        new BlendShapeWeight(BlendShape.JawOpen, 0.6f),
        new BlendShapeWeight(BlendShape.MouthFunnel, 0.3f),
        new BlendShapeWeight(BlendShape.MouthLowerDownLeft, 0.2f),
        new BlendShapeWeight(BlendShape.MouthLowerDownRight, 0.2f)
    };

    [SerializeField]
    private BlendShapeWeight[] closedMouthWeights = new[]
    {
        new BlendShapeWeight(BlendShape.MouthClose, 0.5f),
        new BlendShapeWeight(BlendShape.MouthPressLeft, 0.2f),
        new BlendShapeWeight(BlendShape.MouthPressRight, 0.2f)
    };

    private float[] _samples = new float[256];
    private float _currentAmplitude;
    private float _smoothedAmplitude;
    private BlendShapeWeight[] _currentExpression;

    private void Start()
    {
        _currentExpression = new BlendShapeWeight[openMouthWeights.Length + closedMouthWeights.Length];
    }

    private void Update()
    {
        if (!audioSource.isPlaying)
        {
            ResetMouth();
            return;
        }

        AnalyzeAudio();
        UpdateMouthShape();
    }

    [ContextMenu("Play")]
    public void Play()
    {
        PlayAudioWithLipSync(audioClip, EmotionType.Neutral);
    }

    public void PlayAudioWithLipSync(AudioClip clip, EmotionType emotion = EmotionType.Neutral)
    {
        audioSource.clip = clip;
        audioSource.Play();
        facialController.SetEmotion(emotion, 0.1f, 0.1f);
    }

    private void AnalyzeAudio()
    {
        audioSource.GetOutputData(_samples, 0);

        float sum = 0;
        for (int i = 0; i < _samples.Length; i++)
        {
            sum += Mathf.Abs(_samples[i]);
        }

        _currentAmplitude = sum / _samples.Length * sensitivity;
        _smoothedAmplitude = Mathf.Lerp(_smoothedAmplitude, _currentAmplitude, smoothSpeed * Time.deltaTime);
    }

    private void UpdateMouthShape()
    {
        float normalizedAmplitude = responseCurve.Evaluate(Mathf.Clamp01(_smoothedAmplitude));

        if (normalizedAmplitude < minThreshold)
        {
            ApplyClosedMouth(1f - normalizedAmplitude);
            return;
        }

        ApplyOpenMouth(normalizedAmplitude);
    }

    private void ApplyOpenMouth(float intensity)
    {
        var expressions = new BlendShapeWeight[openMouthWeights.Length];

        for (int i = 0; i < openMouthWeights.Length; i++)
        {
            expressions[i] = new BlendShapeWeight(
                openMouthWeights[i].BlendShape,
                openMouthWeights[i].Weight * intensity
            );
        }

        facialController.SetCustomExpression(expressions, 0.05f);
    }

    private void ApplyClosedMouth(float intensity)
    {
        var expressions = new BlendShapeWeight[closedMouthWeights.Length];

        for (int i = 0; i < closedMouthWeights.Length; i++)
        {
            expressions[i] = new BlendShapeWeight(
                closedMouthWeights[i].BlendShape,
                closedMouthWeights[i].Weight * intensity
            );
        }

        facialController.SetCustomExpression(expressions, 0.1f);
    }

    private void ResetMouth()
    {
        _smoothedAmplitude = 0;
        facialController.ResetToNeutral(0.3f);
    }
}