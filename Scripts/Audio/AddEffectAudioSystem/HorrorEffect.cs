using System;
using UnityEngine;

internal class HorrorEffect : IAudioEffect
{
    private readonly HorrorEffectParameters _params;
    private float[] _reverseBuffer;
    private int _reverseIndex;
    private int _reverseSize;
    private System.Random _random;
    private float _pitchPhase;
    private bool _isReversing;
    private int _glitchCounter;

    public HorrorEffect(HorrorEffectParameters parameters)
    {
        _params = parameters;
        _random = new System.Random();
    }

    public void Initialize(int sampleRate, int channels)
    {
        _reverseSize = sampleRate / 10; // 100ms буфер
        _reverseBuffer = new float[_reverseSize * channels];
        _reverseIndex = 0;
    }

    public void Process(float[] data, int channels)
    {
        for (int i = 0; i < data.Length; i += channels)
        {
            // Случайные глитчи
            if (_random.NextDouble() < _params.glitchProbability * 0.001f)
            {
                _glitchCounter = _random.Next(10, 50);
            }

            // Случайное реверсирование
            if (!_isReversing && _random.NextDouble() < _params.reverseChance * 0.0001f)
            {
                _isReversing = true;
                _reverseIndex = 0;
            }

            for (int ch = 0; ch < channels; ch++)
            {
                float sample = data[i + ch];

                // Применяем глитч
                if (_glitchCounter > 0)
                {
                    sample = (float)(_random.NextDouble() * 2 - 1) * 0.8f;
                    _glitchCounter--;
                }

                // Pitch shifting через изменение фазы
                float pitchShift = Mathf.Sin(_pitchPhase) * _params.pitchShiftRange;
                _pitchPhase += 0.01f;
                sample *= Mathf.Pow(2f, pitchShift / 12f);

                // Добавляем "шепот" (высокочастотный шум)
                float whisper = (float)(_random.NextDouble() * 2 - 1) * _params.whisperMix * 0.1f;
                sample += whisper;

                // Реверсирование
                if (_isReversing)
                {
                    int bufferPos = (_reverseIndex * channels + ch) % _reverseBuffer.Length;
                    _reverseBuffer[bufferPos] = sample;

                    if (_reverseIndex > _reverseSize / 2)
                    {
                        int readPos = ((_reverseSize - _reverseIndex) * channels + ch) % _reverseBuffer.Length;
                        sample = _reverseBuffer[readPos];
                    }
                }

                data[i + ch] = Mathf.Clamp(sample, -1f, 1f);
            }

            if (_isReversing)
            {
                _reverseIndex++;
                if (_reverseIndex >= _reverseSize)
                {
                    _isReversing = false;
                    _reverseIndex = 0;
                }
            }
        }
    }

    public void Reset()
    {
        _reverseIndex = 0;
        _pitchPhase = 0f;
        _isReversing = false;
        _glitchCounter = 0;
        if (_reverseBuffer != null)
            Array.Clear(_reverseBuffer, 0, _reverseBuffer.Length);
    }
}