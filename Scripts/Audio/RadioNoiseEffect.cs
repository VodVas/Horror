using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class RadioNoiseEffect : MonoBehaviour
{
    [SerializeField] private float _noiseVolume = 0.1f;
    [SerializeField] private float _cutoffFrequency = 1000f;

    private AudioSource _audioSource;
    private System.Random _random = new System.Random();

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioSource.clip = CreateNoiseClip(1);
        _audioSource.loop = true;
        _audioSource.Play();
    }

    private AudioClip CreateNoiseClip(int lengthSeconds)
    {
        int samples = lengthSeconds * 44100;
        float[] data = new float[samples];

        for (int i = 0; i < samples; i++)
        {
            data[i] = (float)(_random.NextDouble() * 2 - 1) * _noiseVolume;
        }

        AudioClip clip = AudioClip.Create("Noise", samples, 1, 44100, false);
        clip.SetData(data, 0);
        return clip;
    }
}