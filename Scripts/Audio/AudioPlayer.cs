using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _audioClip;

    private void Awake()
    {
        if (_audioSource == null || _audioClip == null)
        {
            Debug.Log("Audio not assign", this);
            enabled = false;
            return;
        }
    }

    public float GetClipLength()
    {
        return _audioClip.length;
    }

    public AudioClip GetAudioClip()
    {
        return _audioClip;
    }

    public void Play()
    {
        _audioSource.PlayOneShot(_audioClip);
        //_audioSource.Play();
    }

    public void Stop()
    {
        _audioSource.Stop();
    }
}