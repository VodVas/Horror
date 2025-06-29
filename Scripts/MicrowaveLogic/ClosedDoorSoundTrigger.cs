using UnityEngine;

public class ClosedDoorSoundTrigger : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _audioClip;

    private void Awake()
    {
        if (_audioClip == null || _audioSource == null)
        {
            Debug.Log("Dependencies not assign", this);
            enabled = false;
            return;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out ClosedDoorSoundCollider _))
        {
            _audioSource.PlayOneShot(_audioClip);
        }
    }
}