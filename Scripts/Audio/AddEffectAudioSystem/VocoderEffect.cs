using UnityEngine;

internal class VocoderEffect : IAudioEffect
{
    private readonly VocoderEffectParameters _params;
    private ButterworthFilter[] _analysisFilters;
    private ButterworthFilter[] _synthesisFilters;
    private float[] _envelopes;
    private float _carrierPhase;
    private int _sampleRate;

    public VocoderEffect(VocoderEffectParameters parameters)
    {
        _params = parameters;
    }

    public void Initialize(int sampleRate, int channels)
    {
        _sampleRate = sampleRate;
        _analysisFilters = new ButterworthFilter[_params.bandCount];
        _synthesisFilters = new ButterworthFilter[_params.bandCount];
        _envelopes = new float[_params.bandCount];

        float minFreq = 100f;
        float maxFreq = 4000f;
        float freqStep = (maxFreq - minFreq) / _params.bandCount;

        for (int i = 0; i < _params.bandCount; i++)
        {
            float freq = minFreq + i * freqStep;
            _analysisFilters[i] = new ButterworthFilter();
            _synthesisFilters[i] = new ButterworthFilter();
            _analysisFilters[i].SetLowPass(freq + freqStep * 0.5f, sampleRate);
            _synthesisFilters[i].SetLowPass(freq + freqStep * 0.5f, sampleRate);

            if (i > 0)
            {
                _analysisFilters[i].SetHighPass(freq - freqStep * 0.5f, sampleRate);
                _synthesisFilters[i].SetHighPass(freq - freqStep * 0.5f, sampleRate);
            }
        }
    }

    public void Process(float[] data, int channels)
    {
        float carrierIncrement = 2f * Mathf.PI * _params.carrierFrequency / _sampleRate;

        for (int i = 0; i < data.Length; i += channels)
        {
            float carrier = Mathf.Sin(_carrierPhase) * 0.5f;
            _carrierPhase += carrierIncrement;
            if (_carrierPhase > 2f * Mathf.PI) _carrierPhase -= 2f * Mathf.PI;

            for (int ch = 0; ch < channels; ch++)
            {
                float input = data[i + ch];
                float output = 0f;

                // Анализ входного сигнала по полосам
                for (int band = 0; band < _params.bandCount; band++)
                {
                    float filtered = _analysisFilters[band].Process(input);
                    _envelopes[band] = Mathf.Abs(filtered) * 0.1f + _envelopes[band] * 0.9f;
                }

                // Синтез с использованием carrier
                for (int band = 0; band < _params.bandCount; band++)
                {
                    float modulated = _synthesisFilters[band].Process(carrier * _envelopes[band]);
                    output += modulated;
                }

                data[i + ch] = Mathf.Lerp(input, output * 0.5f, _params.carrierMix);
            }
        }
    }

    public void Reset()
    {
        _carrierPhase = 0f;
        for (int i = 0; i < _params.bandCount; i++)
        {
            _analysisFilters[i]?.Reset();
            _synthesisFilters[i]?.Reset();
            _envelopes[i] = 0f;
        }
    }
}