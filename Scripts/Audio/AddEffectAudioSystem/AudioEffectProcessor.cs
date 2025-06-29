using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioEffectProcessor : MonoBehaviour
{
    [SerializeField] private AudioEffectPreset _defaultPreset;
    [SerializeField] private bool _bypassEffect = false;
    [SerializeField] private float _wetDryMix = 1f;

    private IAudioEffect _currentEffect;
    private AudioEffectPreset _currentPreset;
    private int _sampleRate;
    private int _channels;
    private float[] _dryBuffer;
    private readonly object _effectLock = new object();

    private void Awake()
    {
        _sampleRate = AudioSettings.outputSampleRate;
        var config = AudioSettings.GetConfiguration();
        _channels = (int)config.speakerMode;

        if (_defaultPreset != null)
        {
            ApplyPreset(_defaultPreset);
        }
    }

    private void OnAudioFilterRead(float[] data, int channels)
    {
        if (_bypassEffect || _currentEffect == null)
            return;

        lock (_effectLock)
        {
            // Сохраняем оригинальный сигнал для dry/wet микса
            if (_wetDryMix < 1f)
            {
                if (_dryBuffer == null || _dryBuffer.Length != data.Length)
                    _dryBuffer = new float[data.Length];

                System.Array.Copy(data, _dryBuffer, data.Length);
            }

            // Применяем эффект
            _currentEffect.Process(data, channels);

            // Применяем dry/wet микс
            if (_wetDryMix < 1f)
            {
                for (int i = 0; i < data.Length; i++)
                {
                    data[i] = data[i] * _wetDryMix + _dryBuffer[i] * (1f - _wetDryMix);
                }
            }
        }
    }

    public void ApplyPreset(AudioEffectPreset preset)
    {
        if (preset == null) return;

        lock (_effectLock)
        {
            _currentPreset = preset;
            _currentEffect?.Reset();
            _currentEffect = preset.CreateEffect();
            _currentEffect?.Initialize(_sampleRate, _channels);
        }
    }

    public void SetBypass(bool bypass)
    {
        _bypassEffect = bypass;
    }

    public void SetWetDryMix(float mix)
    {
        _wetDryMix = Mathf.Clamp01(mix);
    }

    private void OnDestroy()
    {
        lock (_effectLock)
        {
            _currentEffect?.Reset();
            _currentEffect = null;
        }
    }
}