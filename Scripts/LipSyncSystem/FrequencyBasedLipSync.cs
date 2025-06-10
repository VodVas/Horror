using UnityEngine;

public class FrequencyBasedLipSync : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private FacialExpressionController facialController;
    [SerializeField] private AudioSource audioSource;

    [Header("Frequency Settings")]
    [SerializeField] private FFTWindow fftWindow = FFTWindow.BlackmanHarris;
    [SerializeField] private int spectrumSize = 512;
    [SerializeField] private float lowFreqThreshold = 200f;
    [SerializeField] private float midFreqThreshold = 2000f;
    [SerializeField] private float highFreqThreshold = 4000f;

    [Header("Blend Settings")]
    //[SerializeField] private float blendSpeed = 15f;
    [SerializeField] private float noiseFloor = 0.001f;

    private float[] _spectrum;
    private float _sampleRate;

    private readonly BlendShape[] _lowFreqShapes =
    {
        BlendShape.JawOpen, BlendShape.MouthFunnel,
        BlendShape.MouthLowerDownLeft, BlendShape.MouthLowerDownRight
    };

    private readonly BlendShape[] _midFreqShapes =
    {
        BlendShape.MouthSmileLeft, BlendShape.MouthSmileRight,
        BlendShape.MouthStretchLeft, BlendShape.MouthStretchRight
    };

    private readonly BlendShape[] _highFreqShapes =
    {
        BlendShape.MouthPucker, BlendShape.MouthDimpleLeft,
        BlendShape.MouthDimpleRight, BlendShape.MouthUpperUpLeft
    };

    private void Start()
    {
        _spectrum = new float[spectrumSize];
        _sampleRate = AudioSettings.outputSampleRate;
    }

    private void Update()
    {
        if (!audioSource.isPlaying) return;

        AnalyzeSpectrum();
        ApplyFrequencyBasedExpression();
    }

    private void AnalyzeSpectrum()
    {
        audioSource.GetSpectrumData(_spectrum, 0, fftWindow);
    }

    private void ApplyFrequencyBasedExpression()
    {
        float lowFreqPower = GetFrequencyPower(0, lowFreqThreshold);
        float midFreqPower = GetFrequencyPower(lowFreqThreshold, midFreqThreshold);
        float highFreqPower = GetFrequencyPower(midFreqThreshold, highFreqThreshold);

        var expressions = new System.Collections.Generic.List<BlendShapeWeight>();

        if (lowFreqPower > noiseFloor)
        {
            foreach (var shape in _lowFreqShapes)
            {
                expressions.Add(new BlendShapeWeight(shape, lowFreqPower * 0.8f));
            }
        }

        if (midFreqPower > noiseFloor)
        {
            foreach (var shape in _midFreqShapes)
            {
                expressions.Add(new BlendShapeWeight(shape, midFreqPower * 0.6f));
            }
        }

        if (highFreqPower > noiseFloor)
        {
            foreach (var shape in _highFreqShapes)
            {
                expressions.Add(new BlendShapeWeight(shape, highFreqPower * 0.4f));
            }
        }

        if (expressions.Count > 0)
        {
            facialController.SetCustomExpression(expressions.ToArray(), 0.05f);
        }
    }

    private float GetFrequencyPower(float minFreq, float maxFreq)
    {
        int minBin = Mathf.FloorToInt(minFreq * spectrumSize / (_sampleRate * 0.5f));
        int maxBin = Mathf.CeilToInt(maxFreq * spectrumSize / (_sampleRate * 0.5f));

        minBin = Mathf.Clamp(minBin, 0, spectrumSize - 1);
        maxBin = Mathf.Clamp(maxBin, 0, spectrumSize - 1);

        float power = 0;
        for (int i = minBin; i <= maxBin; i++)
        {
            power += _spectrum[i];
        }

        return power / (maxBin - minBin + 1);
    }
}