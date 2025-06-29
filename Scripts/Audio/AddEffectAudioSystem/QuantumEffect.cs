using System;
using UnityEngine;

internal class QuantumEffect : IAudioEffect
{
    private readonly QuantumEffectParameters _params;
    private float[] _stretchBuffer;
    private int _stretchIndex;
    private float _modulationPhase;
    private float _readPosition;
    private int _sampleRate;

    public QuantumEffect(QuantumEffectParameters parameters)
    {
        _params = parameters;
    }

    public void Initialize(int sampleRate, int channels)
    {
        _sampleRate = sampleRate;
        int bufferSize = sampleRate * 2; // 2 секунды буфер
        _stretchBuffer = new float[bufferSize * channels];
        _stretchIndex = 0;
        _readPosition = 0f;
    }

    public void Process(float[] data, int channels)
    {
        float modIncrement = 2f * Mathf.PI * _params.modulationRate / _sampleRate;

        for (int i = 0; i < data.Length; i += channels)
        {
            // Модуляция
            float modulation = Mathf.Sin(_modulationPhase) * _params.modulationDepth;
            _modulationPhase += modIncrement;

            for (int ch = 0; ch < channels; ch++)
            {
                float sample = data[i + ch];

                // Записываем в буфер
                int writePos = (_stretchIndex * channels + ch) % _stretchBuffer.Length;
                _stretchBuffer[writePos] = sample;

                // Time stretch с pitch shift
                float stretchRate = _params.timeStretch * Mathf.Pow(2f, _params.pitchShift / 12f);
                _readPosition += stretchRate;

                // Интерполяция для плавного растяжения
                int readIndex = (int)_readPosition;
                float fraction = _readPosition - readIndex;

                if (readIndex >= 0 && readIndex < _stretchBuffer.Length / channels - 1)
                {
                    int pos1 = (readIndex * channels + ch) % _stretchBuffer.Length;
                    int pos2 = ((readIndex + 1) * channels + ch) % _stretchBuffer.Length;
                    sample = _stretchBuffer[pos1] * (1f - fraction) + _stretchBuffer[pos2] * fraction;
                }

                // Применяем модуляцию к громкости и панораме
                sample *= 1f + modulation * 0.5f;

                // Добавляем фазовый сдвиг для "квантового" эффекта
                float phasedSample = sample * Mathf.Cos(modulation * Mathf.PI);
                sample = Mathf.Lerp(sample, phasedSample, 0.5f);

                data[i + ch] = Mathf.Clamp(sample, -1f, 1f);
            }

            _stretchIndex = (_stretchIndex + 1) % (_stretchBuffer.Length / channels);

            // Зацикливание позиции чтения
            if (_readPosition >= _stretchBuffer.Length / channels)
                _readPosition -= _stretchBuffer.Length / channels;
        }
    }

    public void Reset()
    {
        _stretchIndex = 0;
        _readPosition = 0f;
        _modulationPhase = 0f;
        if (_stretchBuffer != null)
            Array.Clear(_stretchBuffer, 0, _stretchBuffer.Length);
    }
}