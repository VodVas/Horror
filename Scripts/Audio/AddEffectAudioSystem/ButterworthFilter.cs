using UnityEngine;

internal class ButterworthFilter
{
    private float _a0, _a1, _a2, _b1, _b2;
    private float _z1, _z2;

    public void SetLowPass(float frequency, float sampleRate)
    {
        float w = 2f * Mathf.PI * frequency / sampleRate;
        float cosw = Mathf.Cos(w);
        float sinw = Mathf.Sin(w);
        float alpha = sinw / Mathf.Sqrt(2f);

        float a0 = 1f + alpha;
        _b1 = (1f - cosw) / a0;
        _a0 = _b1 * 0.5f;
        _a1 = _b1;
        _a2 = _a0;
        _b1 = 2f * cosw / a0;
        _b2 = (1f - alpha) / a0;
    }

    public void SetHighPass(float frequency, float sampleRate)
    {
        float w = 2f * Mathf.PI * frequency / sampleRate;
        float cosw = Mathf.Cos(w);
        float sinw = Mathf.Sin(w);
        float alpha = sinw / Mathf.Sqrt(2f);

        float a0 = 1f + alpha;
        _a0 = (1f + cosw) / (2f * a0);
        _a1 = -(1f + cosw) / a0;
        _a2 = _a0;
        _b1 = 2f * cosw / a0;
        _b2 = (1f - alpha) / a0;
    }

    public float Process(float input)
    {
        float output = _a0 * input + _a1 * _z1 + _a2 * _z2 + _b1 * _z1 + _b2 * _z2;
        _z2 = _z1;
        _z1 = input - _b1 * _z1 - _b2 * _z2;
        return output;
    }

    public void Reset()
    {
        _z1 = _z2 = 0f;
    }
}