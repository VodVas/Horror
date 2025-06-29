using System;
using UnityEngine;

internal class UnderwaterEffect : IAudioEffect
{
    private readonly UnderwaterEffectParameters _params;
    private ButterworthFilter _lowPass;
    private float[] _delayBuffer;
    private int _delayIndex;
    private float _wobblePhase;
    private int _sampleRate;

    public UnderwaterEffect(UnderwaterEffectParameters parameters)
    {
        _params = parameters;
    }

    public void Initialize(int sampleRate, int channels)
    {
        _sampleRate = sampleRate;
        _lowPass = new ButterworthFilter();
        _lowPass.SetLowPass(_params.cutoffFrequency, sampleRate);

        int delaySize = (int)(sampleRate * _params.echoDelay);
        _delayBuffer = new float[delaySize * channels];
        _delayIndex = 0;
    }

    public void Process(float[] data, int channels)
    {
        float wobbleIncrement = 2f * Mathf.PI * 0.5f / _sampleRate;

        for (int i = 0; i < data.Length; i += channels)
        {
            // Модуляция частоты среза фильтра
            float wobble = Mathf.Sin(_wobblePhase) * _params.wobbleAmount;
            _wobblePhase += wobbleIncrement;

            for (int ch = 0; ch < channels; ch++)
            {
                float sample = data[i + ch];

                // Применяем low-pass фильтр
                sample = _lowPass.Process(sample);

                // Добавляем эхо
                if (_delayBuffer != null && _delayBuffer.Length > 0)
                {
                    int delayPos = (_delayIndex + ch) % _delayBuffer.Length;
                    float delayed = _delayBuffer[delayPos];
                    _delayBuffer[delayPos] = sample + delayed * _params.echoDecay;
                    sample += delayed * 0.5f;
                }

                // Применяем wobble к громкости
                sample *= 1f + wobble * 0.2f;

                data[i + ch] = Mathf.Clamp(sample, -1f, 1f);
            }

            _delayIndex = (_delayIndex + channels) % _delayBuffer.Length;
        }
    }

    public void Reset()
    {
        _lowPass?.Reset();
        _wobblePhase = 0f;
        _delayIndex = 0;
        if (_delayBuffer != null)
            Array.Clear(_delayBuffer, 0, _delayBuffer.Length);
    }
}