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
   // [SerializeField] private GameObject _monster;
    [SerializeField] private EmoSetter _emoSetter;
    [SerializeField] private AudioClip _ambientSound;
    [SerializeField] private HybridLipSync _hybridLipSync;
    [SerializeField] private FoodValidator _foodValidator;

    private int _hitCount = 0;

    public event Action EndOf1Scene;
    public event Action EndOf2Scene;

    private void OnEnable()
    {
        if (_foodValidator != null)
        {
            _foodValidator.OnValidFoodDetected += HandleValidFood;
        }
    }

    private void OnDisable()
    {
        if (_foodValidator != null)
        {
            _foodValidator.OnValidFoodDetected -= HandleValidFood;
        }
    }

    private void HandleValidFood(GameObject foodObject)
    {
        _hitCount++;
        Debug.Log($"Hit count: {_hitCount}");

        if (_hitCount == _requiredHitsFirstCutscene)
        {
            //StartCoroutine(PlayAndHide1CutsceneAfterDelay());
        }

        if (_hitCount == _requiredHitsSecondCutscene)
        {
            StartCoroutine(WaitAndRun());
        }
    }

    //private IEnumerator PlayAndHide1CutsceneAfterDelay()
    //{
    //    _cutsceneObject.gameObject.SetActive(true);

    //    bool hasAudioListener = _cameraObject.TryGetComponent(out AudioListener audioListener);

    //    if (hasAudioListener)
    //    {
    //        _waypointMover[0].enabled = false;
    //        audioListener.enabled = false;

    //        yield return new WaitForSeconds(_cutsceneDuration);

    //        audioListener.enabled = true;
    //    }

    //    _cutsceneObject.gameObject.SetActive(false);

    //    EndOf1Scene?.Invoke();
    //}

    private IEnumerator WaitAndRun()
    {
        _audioSource.Play();
        yield return null;

        //_monster?.SetActive(true);
        _roundSoundPlayer.SetActive(true);
        _emoSetter.SetEnterEmotion();
        yield return new WaitForSeconds(_roundSoundPlayerDuration);

        _hybridLipSync.StartLipSync(_daveSpeech);
        yield return new WaitForSeconds(_daveSpeech.length + 0.5f);
        EndOf1Scene?.Invoke();

       // _monster?.SetActive(false);
        _animator?.SetBool(IsRunning, true);
        _waypointMover[1].enabled = true;

        yield return new WaitForSeconds(0.5f);
        _cutsceneTimeline.SetActive(true);

        yield return new WaitForSeconds(0.8f);

        _cutsceneTimeline.SetActive(false);
        EndOf2Scene?.Invoke();
    }
}