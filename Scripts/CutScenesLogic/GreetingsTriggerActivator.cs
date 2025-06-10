using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class GreetingsTriggerActivator : MonoBehaviour
{
    [SerializeField] private EmoSetter _emoSetter;
    [SerializeField] private AudioPlayer _audioPlayer;
    [SerializeField] private GameObject _secondCutsceneActivator;
    [SerializeField] private BoxCollider _daveBodyCollider;
    [SerializeField] private BoxCollider _daveHeadCollider;
    [SerializeField] private HybridLipSync _hybridLipSync;

    private bool _isGreetingsEnabled = true;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Dave _))
        {
            if (_isGreetingsEnabled)
            {
                _emoSetter.SetEnterEmotion();
                _hybridLipSync.StartLipSync(_audioPlayer.GetAudioClip());
                StartCoroutine(Delay(_audioPlayer.GetClipLenght()));
                _secondCutsceneActivator.SetActive(true);
                _isGreetingsEnabled = false;
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

    private IEnumerator Delay(float delay)
    {
        yield return new WaitForSeconds(delay);

        _daveBodyCollider.isTrigger = true;
        _daveHeadCollider.isTrigger = true;
    }
}
