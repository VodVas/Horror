using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class GiggleSceneLogic : MonoBehaviour
{
    [Serializable]
    private class GiggleEntity
    {
        [SerializeField] private ObjectOnceShaker _shaker;
        [SerializeField] private AudioPlayer _audioPlayer;

        public ObjectOnceShaker Shaker => _shaker;
        public AudioPlayer AudioPlayer => _audioPlayer;
    }

    [SerializeField] private GameObject _lamps;
    [SerializeField] private GameObject _lights;
    [SerializeField] private GameObject _dave;
    [SerializeField] private GameObject _microwaveGiggleSoundTrigger;
    [SerializeField] private GameObject _roundSoundPlayer1;
    [SerializeField] private GameObject _cutScene;
    [SerializeField] private GameObject _cutSceneCamera;
    [SerializeField] private GameObject _ambientMusic;
    [SerializeField] private HauntedDoor _microwaveDoor;
    [SerializeField] private float _delayBeforeStartScene = 10f;
    [SerializeField] private float _delayBeforeScene = 10f;
    [SerializeField] private float _delayBeforeStartShake = 4f;
    [SerializeField] private float _delayBeforeDaveActivate = 10f;
    [SerializeField] private float _delayBeforeSpeedUpGiggle = 4f;
    [SerializeField] private float _delayBeforeStop = 4f;
    [SerializeField] private float _delayBeforeDeactivateCamera = 3f;
    [SerializeField] private AudioPlayer _microwaveAudioPlayer;
    [SerializeField] private VerticalPingPongMover[] _objectMovers;
    [SerializeField] private float _delayBetweenActivateMove = 0.5f;
    [SerializeField] private GiggleEntity[] _giggleEntities;
    [SerializeField] private float _delayBetweenShake = 0.5f;

    [SerializeField] private HeadBeatLogic _headBeatLogic;

    public event Action EndOfScene;

    private void Awake()
    {
        bool allFieldsValid = true;

        if (_lamps == null)
        {
            Debug.LogError($"{nameof(_lamps)} is not assigned!", this);
            allFieldsValid = false;
        }
        if (_lights == null)
        {
            Debug.LogError($"{nameof(_lights)} is not assigned!", this);
            allFieldsValid = false;
        }
        if (_dave == null)
        {
            Debug.LogError($"{nameof(_dave)} is not assigned!", this);
            allFieldsValid = false;
        }
        if (_microwaveGiggleSoundTrigger == null)
        {
            Debug.LogError($"{nameof(_microwaveGiggleSoundTrigger)} is not assigned!", this);
            allFieldsValid = false;
        }
        if (_roundSoundPlayer1 == null)
        {
            Debug.LogError($"{nameof(_roundSoundPlayer1)} is not assigned!", this);
            allFieldsValid = false;
        }
        if (_microwaveDoor == null)
        {
            Debug.LogError($"{nameof(_microwaveDoor)} is not assigned!", this);
            allFieldsValid = false;
        }
        if (_microwaveAudioPlayer == null)
        {
            Debug.LogError($"{nameof(_microwaveAudioPlayer)} is not assigned!", this);
            allFieldsValid = false;
        }

        if (_objectMovers == null || _objectMovers.Length == 0)
        {
            Debug.LogError($"{nameof(_objectMovers)} array is not assigned or empty!", this);
            allFieldsValid = false;
        }
        else
        {
            for (int i = 0; i < _objectMovers.Length; i++)
            {
                if (_objectMovers[i] == null)
                {
                    Debug.LogError($"{nameof(_objectMovers)} element at index {i} is null!", this);
                    allFieldsValid = false;
                }
            }
        }

        if (_giggleEntities == null || _giggleEntities.Length == 0)
        {
            Debug.LogError($"{nameof(_giggleEntities)} array is not assigned or empty!", this);
            allFieldsValid = false;
        }
        else
        {
            for (int i = 0; i < _giggleEntities.Length; i++)
            {
                if (_giggleEntities[i] == null)
                {
                    Debug.LogError($"{nameof(_giggleEntities)} element at index {i} is null!", this);
                    allFieldsValid = false;
                }
                else
                {
                    if (_giggleEntities[i].Shaker == null)
                    {
                        Debug.LogError($"{nameof(GiggleEntity.Shaker)} in {nameof(_giggleEntities)}[{i}] is not assigned!", this);
                        allFieldsValid = false;
                    }
                    if (_giggleEntities[i].AudioPlayer == null)
                    {
                        Debug.LogError($"{nameof(GiggleEntity.AudioPlayer)} in {nameof(_giggleEntities)}[{i}] is not assigned!", this);
                        allFieldsValid = false;
                    }
                }
            }
        }

        if (!allFieldsValid)
        {
            Debug.LogError("Not all fields are properly assigned in the inspector!", this);
            enabled = false;
        }
    }

    private void OnEnable()
    {
        _headBeatLogic.EndOfHeadBeatScene += StartScene;
    }

    private void OnDisable()
    {
        _headBeatLogic.EndOfHeadBeatScene -= StartScene;
    }

    private void StartScene()
    {
        Execute().Forget();
    }

    [ContextMenu("Execute")]
    private async UniTaskVoid Execute()
    {
        var ct = this.GetCancellationTokenOnDestroy();
        await UniTask.Delay(TimeSpan.FromSeconds(_delayBeforeStartScene), ignoreTimeScale: false, cancellationToken: ct);

        _lamps.SetActive(true);
        await UniTask.Delay(TimeSpan.FromSeconds(_delayBeforeScene), ignoreTimeScale: false, cancellationToken: ct);
        _microwaveGiggleSoundTrigger.SetActive(true);
        _microwaveDoor.StartHaunting();
        _microwaveAudioPlayer.Play();
        await UniTask.Delay(TimeSpan.FromSeconds(_delayBeforeDaveActivate), ignoreTimeScale: false, cancellationToken: ct);
        _dave.SetActive(true);
        await UniTask.Delay(TimeSpan.FromSeconds(_delayBeforeStartShake), ignoreTimeScale: false, cancellationToken: ct);
        _microwaveDoor.SetDuration(0.7f);
        _microwaveDoor.SetPauseDuration(0.7f);
        StartVerticalMoverSequence().Forget();
        SequenceShake().Forget();
        await UniTask.Delay(TimeSpan.FromSeconds(_delayBeforeSpeedUpGiggle), ignoreTimeScale: false, cancellationToken: ct);
        _microwaveDoor.SetDuration(0.2f);
        _microwaveDoor.SetPauseDuration(0.2f);
        VerticalMoverSpeedUp().Forget();
        await UniTask.Delay(TimeSpan.FromSeconds(_delayBeforeSpeedUpGiggle), ignoreTimeScale: false, cancellationToken: ct);
        _microwaveDoor.SetDuration(0.05f);
        _microwaveDoor.SetPauseDuration(0.05f);
        _cutSceneCamera.SetActive(true);
        DisableCameraAfterDelay(_delayBeforeDeactivateCamera).Forget();
        _cutScene.SetActive(true);
        _roundSoundPlayer1.SetActive(true);
        await UniTask.Delay(TimeSpan.FromSeconds(_delayBeforeStop), ignoreTimeScale: false, cancellationToken: ct);
        _dave.SetActive(false);
        _cutScene.SetActive(false);
        _microwaveDoor.StopHaunting();
        StopVerticalMoverSequence();
        StopGiggleSequenceAsync();
        _roundSoundPlayer1.SetActive(false);
        _lights.SetActive(true);
        _ambientMusic.SetActive(false);

        EndOfScene?.Invoke();
    }

    private async UniTaskVoid DisableCameraAfterDelay(float delay)
    {
        try
        {
            var ct = this.GetCancellationTokenOnDestroy();
            await UniTask.Delay(TimeSpan.FromSeconds(delay), cancellationToken: ct);
            _cutSceneCamera.SetActive(false);
        }
        catch
        {
            throw;
        }
    }

    private async UniTask SequenceShake()
    {
        foreach (var entity in _giggleEntities)
        {
            var ct = this.GetCancellationTokenOnDestroy();

            entity.Shaker.Shake();
            entity.AudioPlayer.Play();

            await UniTask.Delay(TimeSpan.FromSeconds(_delayBetweenShake), cancellationToken: ct);
        }
    }

    private void StopGiggleSequenceAsync()
    {
        foreach (var entity in _giggleEntities)
        {
            entity.Shaker.StopShake();
            entity.AudioPlayer.Stop();
        }
    }

    private async UniTask StartVerticalMoverSequence()
    {
        var ct = this.GetCancellationTokenOnDestroy();

        foreach (var mover in _objectMovers)
        {
            mover.StartMovement();

            await UniTask.Delay(TimeSpan.FromSeconds(_delayBetweenActivateMove), cancellationToken: ct);
        }
    }

    private void StopVerticalMoverSequence()
    {
        foreach (var mover in _objectMovers)
        {
            mover.StopAndReset();
        }
    }

    private async UniTask VerticalMoverSpeedUp()
    {
        var ct = this.GetCancellationTokenOnDestroy();

        foreach (var mover in _objectMovers)
        {
            float rundomNumber = UnityEngine.Random.Range(0.5f, 1.5f);

            mover.SetSpeed(rundomNumber);

            await UniTask.Delay(TimeSpan.FromSeconds(_delayBetweenActivateMove), cancellationToken: ct);
        }
    }
}