using UnityEngine;

[CreateAssetMenu(fileName = "NewPhonemicClip", menuName = "Audio/Phonemic Clip")]
public class PhonemicClip : ScriptableObject
{
    [SerializeField] private AudioClip _audioClip;
    [SerializeField] private TimedViseme[] _visemes = new TimedViseme[0];
    [SerializeField] private string _transcript = string.Empty;
    [SerializeField] private float _wordsPerMinute = 150f;

    public AudioClip Clip => _audioClip;
    public TimedViseme[] Visemes => _visemes;
    public string Transcript => _transcript;
    public float WordsPerMinute => _wordsPerMinute;

    public void Initialize(AudioClip clip, string transcript)
    {
        _audioClip = clip;
        _transcript = transcript;
        _visemes = new TimedViseme[0];
    }

    public void SetVisemes(TimedViseme[] visemes)
    {
        _visemes = visemes ?? new TimedViseme[0];
    }

    public void SetWordsPerMinute(float wpm)
    {
        _wordsPerMinute = Mathf.Clamp(wpm, 50f, 300f);
    }
}
