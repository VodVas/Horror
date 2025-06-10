using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _audioClip;
    [SerializeField] private CutsceneActivator _cutsceneActivator;

    private void Awake()
    {
        if (_audioSource == null || _audioClip == null)
        {
            Debug.Log("Audio not assign", this);
            enabled = false;
            return;
        }
    }

    private void OnEnable()
    {
        _cutsceneActivator.EndOf1Scene += PlayOnEvent;
    }

    private void OnDisable()
    {
        _cutsceneActivator.EndOf1Scene -= PlayOnEvent;
    }

    public float GetClipLenght()
    {
        //_audioSource.PlayOneShot(_audioClip);

        return _audioClip.length;
    }

    public AudioClip GetAudioClip()
    {
        return _audioClip;
    }

    public void PlayOnEvent()
    {
        _audioSource.Play();
    }
}