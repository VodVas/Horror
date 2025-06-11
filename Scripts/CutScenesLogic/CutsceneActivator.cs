//using System;
//using System.Collections;
//using UnityEngine;

//public class CutsceneActivator : MonoBehaviour
//{
//    private const string IsRunning = "IsRunning";

//    [Header("First Cutscene")]
//    [SerializeField] private int _requiredHitsFirstCutscene = 3;
//    [SerializeField] private int _requiredHitsSecondCutscene = 6;
//    [SerializeField] private Transform _cutsceneObject;
//    [SerializeField] private GameObject _cameraObject;
//    [SerializeField] private float _cutsceneDuration;

//    [SerializeField] private WaypointMover[] _waypointMover;

//    [Header("Second Cutscene")]
//    [SerializeField] private Animator _animator;
//    [SerializeField] private GameObject _daveObject;
//    [SerializeField] private AudioSource _audioSource;
//    [SerializeField] private AudioClip _daveSpeech;
//    [SerializeField] private GameObject _roundSoundPlayer;
//    [SerializeField] private float _roundSoundPlayerDuration = 5f;
//    [SerializeField] private GameObject _cutsceneTimeline;
//    [SerializeField] private GameObject _monster;
//    [SerializeField] private EmoSetter _emoSetter;
//    [SerializeField] private AudioClip _ambientSound;

//    [SerializeField] private HybridLipSync _hybridLipSync;

//    private int _hitCount = 0;

//    public event Action EndOf1Scene;

//    private void OnTriggerEnter(Collider other)
//    {
//        if (other.TryGetComponent(out Food food) && food is IRewardableFood)
//        {
//            _hitCount++;
//            Debug.Log($"Hit count: {_hitCount}");
//            if (_hitCount == _requiredHitsFirstCutscene)
//            {
//                StartCoroutine(PlayAndHide1CutsceneAfterDelay());
//            }

//            if (_hitCount == _requiredHitsSecondCutscene)
//            {
//                StartCoroutine(WaitAndRun());
//            }
//        }
//    }

//    private IEnumerator PlayAndHide1CutsceneAfterDelay()
//    {
//        _cutsceneObject.gameObject.SetActive(true);

//        bool hasAudioListener = _cameraObject.TryGetComponent(out AudioListener audioListener);

//        if (hasAudioListener)
//        {
//            _waypointMover[0].enabled = false;

//            if (hasAudioListener) audioListener.enabled = false;

//            yield return new WaitForSeconds(_cutsceneDuration);

//            if (hasAudioListener) audioListener.enabled = true;
//        }

//        _cutsceneObject.gameObject.SetActive(false);

//        EndOf1Scene?.Invoke();
//    }

//    private IEnumerator WaitAndRun()
//    {
//        _roundSoundPlayer.SetActive(true);
//        _emoSetter.SetEnterEmotion();
//        yield return new WaitForSeconds(_roundSoundPlayerDuration);

//        //_audioSource.PlayOneShot(_daveSpeech);
//        _hybridLipSync.StartLipSync(_daveSpeech);
//        yield return new WaitForSeconds(_daveSpeech.length + 0.2f);

//        _cutsceneTimeline.SetActive(true);
//        _animator?.SetBool(IsRunning, true);

//        _waypointMover[1].enabled = true;

//        yield return new WaitForSeconds(0.8f);

//        _cutsceneTimeline.SetActive(false);
//        _monster?.SetActive(false);
//    }
//}


using System;
using System.Collections;
using UnityEngine;

public class CutsceneActivator : MonoBehaviour
{
    private const string IsRunning = "IsRunning";

    [Header("First Cutscene")]
    [SerializeField] private int _requiredHitsFirstCutscene = 3;
    [SerializeField] private int _requiredHitsSecondCutscene = 6;
    [SerializeField] private Transform _cutsceneObject;
    [SerializeField] private GameObject _cameraObject;
    [SerializeField] private float _cutsceneDuration;

    [SerializeField] private WaypointMover[] _waypointMover;

    [Header("Second Cutscene")]
    [SerializeField] private Animator _animator;
    [SerializeField] private GameObject _daveObject;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _daveSpeech;
    [SerializeField] private GameObject _roundSoundPlayer;
    [SerializeField] private float _roundSoundPlayerDuration = 5f;
    [SerializeField] private GameObject _cutsceneTimeline;
    [SerializeField] private GameObject _monster;
    [SerializeField] private EmoSetter _emoSetter;
    [SerializeField] private AudioClip _ambientSound;

    [SerializeField] private HybridLipSync _hybridLipSync;

    private int _hitCount = 0;

    public event Action EndOf1Scene;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IRewardableFood rewardableFood) && rewardableFood.IsRewardable())
        {
            _hitCount++;
            Debug.Log($"Hit count: {_hitCount}");
            if (_hitCount == _requiredHitsFirstCutscene)
            {
                StartCoroutine(PlayAndHide1CutsceneAfterDelay());
            }

            if (_hitCount == _requiredHitsSecondCutscene)
            {
                StartCoroutine(WaitAndRun());
            }
        }
    }

    private IEnumerator PlayAndHide1CutsceneAfterDelay()
    {
        _cutsceneObject.gameObject.SetActive(true);

        bool hasAudioListener = _cameraObject.TryGetComponent(out AudioListener audioListener);

        if (hasAudioListener)
        {
            _waypointMover[0].enabled = false;

            if (hasAudioListener) audioListener.enabled = false;

            yield return new WaitForSeconds(_cutsceneDuration);

            if (hasAudioListener) audioListener.enabled = true;
        }

        _cutsceneObject.gameObject.SetActive(false);

        EndOf1Scene?.Invoke();
    }

    private IEnumerator WaitAndRun()
    {
        _roundSoundPlayer.SetActive(true);
        _emoSetter.SetEnterEmotion();
        yield return new WaitForSeconds(_roundSoundPlayerDuration);

        _hybridLipSync.StartLipSync(_daveSpeech);
        yield return new WaitForSeconds(_daveSpeech.length + 0.2f);

        _cutsceneTimeline.SetActive(true);
        _animator?.SetBool(IsRunning, true);

        _waypointMover[1].enabled = true;

        yield return new WaitForSeconds(0.8f);

        _cutsceneTimeline.SetActive(false);
        _monster?.SetActive(false);
    }
}