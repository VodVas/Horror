using UnityEngine;

[RequireComponent(typeof(Collider))]
public class GreetingsTriggerActivator : MonoBehaviour
{
    [SerializeField] private EmoSetter _emoSetter;
    [SerializeField] private AudioPlayer _audioPlayer;
    [SerializeField] private GameObject _secondCutsceneActivator;
    [SerializeField] private BoxCollider _daveBodyCollider;


    private bool _isGreetingsEnabled = true;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Dave _))
        {
            if (_isGreetingsEnabled)
            {
                _emoSetter.SetEnterEmotion();
                _audioPlayer.Play();
                _secondCutsceneActivator.SetActive(true);
                _isGreetingsEnabled = false;
                _daveBodyCollider.isTrigger = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Dave _))
        {
            _emoSetter.SetExitEmotion();
        }
    }
}
