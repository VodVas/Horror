using UnityEngine;

internal class TelephoneEffect : IAudioEffect
{
    private readonly TelephoneEffectParameters _params;
    private ButterworthFilter _lowPass;
    private ButterworthFilter _highPass;
    private System.Random _random;

    public TelephoneEffect(TelephoneEffectParameters parameters)
    {
        _params = parameters;
        _random = new System.Random();
    }

    public void Initialize(int sampleRate, int channels)
    {
        _lowPass = new ButterworthFilter();
        _highPass = new ButterworthFilter();
        _lowPass.SetLowPass(_params.highFrequency, sampleRate);
        _highPass.SetHighPass(_params.lowFrequency, sampleRate);
    }

    public void Process(float[] data, int channels)
    {
        for (int i = 0; i < data.Length; i += channels)
        {
            for (int ch = 0; ch < channels; ch++)
            {
                float sample = data[i + ch];

                // Применяем полосовой фильтр
                sample = _lowPass.Process(sample);
                sample = _highPass.Process(sample);

                // Добавляем небольшое искажение
                sample = Mathf.Sign(sample) * Mathf.Pow(Mathf.Abs(sample), 1f - _params.distortion);

                // Добавляем шум
                sample += (float)(_random.NextDouble() * 2 - 1) * _params.noiseLevel;

                // Усиливаем сигнал для компенсации потерь от фильтрации
                sample *= 2f;

                data[i + ch] = Mathf.Clamp(sample, -1f, 1f);
            }
        }
    }

    public void Reset()
    {
        _lowPass?.Reset();
        _highPass?.Reset();
    }
}