using UnityEngine;

[CreateAssetMenu(fileName = "AudioEffectPreset", menuName = "Audio/Effect Preset")]
public class AudioEffectPreset : ScriptableObject
{
    [SerializeField] private AudioEffectType _effectType = AudioEffectType.None;
    [SerializeField] private RadioEffectParameters _radioParams = new RadioEffectParameters();
    [SerializeField] private VocoderEffectParameters _vocoderParams = new VocoderEffectParameters();
    [SerializeField] private UnderwaterEffectParameters _underwaterParams = new UnderwaterEffectParameters();
    [SerializeField] private TelephoneEffectParameters _telephoneParams = new TelephoneEffectParameters();
    [SerializeField] private HorrorEffectParameters _horrorParams = new HorrorEffectParameters();
    [SerializeField] private QuantumEffectParameters _quantumParams = new QuantumEffectParameters();

    public AudioEffectType EffectType => _effectType;

    public IAudioEffect CreateEffect()
    {
        switch (_effectType)
        {
            case AudioEffectType.Radio:
                return _radioParams.CreateEffect();
            case AudioEffectType.Vocoder:
                return _vocoderParams.CreateEffect();
            case AudioEffectType.Underwater:
                return _underwaterParams.CreateEffect();
            case AudioEffectType.Telephone:
                return _telephoneParams.CreateEffect();
            case AudioEffectType.Horror:
                return _horrorParams.CreateEffect();
            case AudioEffectType.Quantum:
                return _quantumParams.CreateEffect();
            default:
                return null;
        }
    }
}