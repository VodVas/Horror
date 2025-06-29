using UnityEngine;

internal class RadioEffect : IAudioEffect
{
    private readonly RadioEffectParameters _params;
    private ButterworthFilter _lowPass;
    private ButterworthFilter _highPass;
    private System.Random _random;
    private float _phase;

    public RadioEffect(RadioEffectParameters parameters)
    {
        _params = parameters;
        _random = new System.Random();
    }

    public void Initialize(int sampleRate, int channels)
    {
        _lowPass = new ButterworthFilter();
        _highPass = new ButterworthFilter();
        _lowPass.SetLowPass(_params.lowPassFrequency, sampleRate);
        _highPass.SetHighPass(_params.highPassFrequency, sampleRate);
    }

    public void Process(float[] data, int channels)
    {
        for (int i = 0; i < data.Length; i += channels)
        {
            for (int ch = 0; ch < channels; ch++)
            {
                float sample = data[i + ch];

                // Добавляем шум
                sample += (float)(_random.NextDouble() * 2 - 1) * _params.noiseLevel;

                // Применяем фильтры
                sample = _lowPass.Process(sample);
                sample = _highPass.Process(sample);

                // Добавляем легкое искажение
                sample = Mathf.Sign(sample) * Mathf.Pow(Mathf.Abs(sample), 1f - _params.distortion * 0.5f);

                data[i + ch] = Mathf.Clamp(sample, -1f, 1f);
            }
        }
    }

    public void Reset()
    {
        _lowPass?.Reset();
        _highPass?.Reset();
        _phase = 0f;
    }
}