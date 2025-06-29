using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class HeadBeatLogic : MonoBehaviour
{
    private const string IsTurn = "IsTurn";
    private const string IsCrawl = "IsCrawl";

    [SerializeField] private GameObject _player;
    [SerializeField] private GameObject _dave;
    [SerializeField] private GameObject _lamps;
    [SerializeField] private GameObject _crosshair;
    [SerializeField] private GameObject _stepWaypointSound;
    [SerializeField] private GameObject _roarSound;
    [SerializeField] private WaypointMover _stepWaypoint;
    [SerializeField] private float _delayBeforeStartScene = 4f;
    [SerializeField] private float _delayBeforeLight = 5f;
    [SerializeField] private float _delayBeforeCutscene = 7f;
    [SerializeField] private float _delayBeforeTurn = 4f;
    [SerializeField] private float _delayBeforeSay = 4f;
    [SerializeField] private float _delayBeforeEmo = 2f;
    [SerializeField] private float _delayBeforeCrawl = 1f;
    [SerializeField] private float _delayBeforeLightOff = 2f;
    [SerializeField] private float _delayBeforeDeactivateScene = 1f;
    [SerializeField] private LightIntensityAnimator _lightFlicker;
    [SerializeField] private LightIntensityAnimator _lightFlicker1;
    [SerializeField] private GameObject _cutScene;
    [SerializeField] private PreciseLipSyncController _lipSync;
    [SerializeField] private EmoSetter _emoSetter;
    [SerializeField] private Animator _animator;
    [SerializeField] private CutsceneActivator _cutsceneActivator;

    public event Action EndOfHeadBeatScene;

    private void Awake()
    {
        bool allFieldsValid = true;

        if (_player == null)
        {
            Debug.LogError("Player GameObject is not assigned in the inspector!", this);
            allFieldsValid = false;
        }

        if (_dave == null)
        {
            Debug.LogError("Dave GameObject is not assigned in the inspector!", this);
            allFieldsValid = false;
        }

        if (_lamps == null)
        {
            Debug.LogError("Lamps GameObject is not assigned in the inspector!", this);
            allFieldsValid = false;
        }

        if (_crosshair == null)
        {
            Debug.LogError("Crosshair GameObject is not assigned in the inspector!", this);
            allFieldsValid = false;
        }

        if (_stepWaypointSound == null)
        {
            Debug.LogError("StepWaypointSound GameObject is not assigned in the inspector!", this);
            allFieldsValid = false;
        }

        if (_roarSound == null)
        {
            Debug.LogError("RoarSound GameObject is not assigned in the inspector!", this);
            allFieldsValid = false;
        }

        if (_stepWaypoint == null)
        {
            Debug.LogError("StepWaypointMover component is not assigned in the inspector!", this);
            allFieldsValid = false;
        }

        if (_lightFlicker == null)
        {
            Debug.LogError("LightFlicker component is not assigned in the inspector!", this);
            allFieldsValid = false;
        }

        if (_lightFlicker1 == null)
        {
            Debug.LogError("LightFlicker1 component is not assigned in the inspector!", this);
            allFieldsValid = false;
        }

        if (_cutScene == null)
        {
            Debug.LogError("CutScene GameObject is not assigned in the inspector!", this);
            allFieldsValid = false;
        }

        if (_lipSync == null)
        {
            Debug.LogError("LipSyncController component is not assigned in the inspector!", this);
            allFieldsValid = false;
        }

        if (_emoSetter == null)
        {
            Debug.LogError("EmoSetter component is not assigned in the inspector!", this);
            allFieldsValid = false;
        }

        if (_animator == null)
        {
            Debug.LogError("Animator component is not assigned in the inspector!", this);
            allFieldsValid = false;
        }

        if (!allFieldsValid)
        {
            enabled = false;
        }
    }

    private void OnEnable()
    {
        _cutsceneActivator.EndOf2Scene += ActivateScene;
    }

    private void OnDisable()
    {
        _cutsceneActivator.EndOf2Scene -= ActivateScene;
    }

    private void ActivateScene()
    {
        Execute().Forget();
    }

    [ContextMenu("Execute")]
    private async UniTaskVoid Execute()
    {
        var ct = this.GetCancellationTokenOnDestroy();
        await UniTask.Delay(TimeSpan.FromSeconds(_delayBeforeStartScene), ignoreTimeScale: false, cancellationToken: ct);

        _dave.SetActive(true);
        _lamps.SetActive(false);
        await UniTask.Delay(TimeSpan.FromSeconds(_delayBeforeLight), ignoreTimeScale: false, cancellationToken: ct);
        _lightFlicker.StartIntensityAnimation();
        _lightFlicker1.StartIntensityAnimation();
        await UniTask.Delay(TimeSpan.FromSeconds(_delayBeforeCutscene), ignoreTimeScale: false, cancellationToken: ct);
        _player.SetActive(false);
        _crosshair.SetActive(false);
        _cutScene.SetActive(true);
        await UniTask.Delay(TimeSpan.FromSeconds(_delayBeforeTurn), ignoreTimeScale: false, cancellationToken: ct);
        _animator.SetBool(IsTurn, true);
        await UniTask.Delay(TimeSpan.FromSeconds(_delayBeforeSay), ignoreTimeScale: false, cancellationToken: ct);
        _lipSync.Play();
        await UniTask.Delay(TimeSpan.FromSeconds(_delayBeforeEmo), ignoreTimeScale: false, cancellationToken: ct);
        _roarSound.SetActive(true);
        _emoSetter.SetEnterEmotion();
        await UniTask.Delay(TimeSpan.FromSeconds(_delayBeforeCrawl), ignoreTimeScale: false, cancellationToken: ct);
        _animator.SetBool(IsTurn, false);
        _animator.SetBool(IsCrawl, true);
        _player.SetActive(true);
        _crosshair.SetActive(true);
        _stepWaypoint.enabled = true;
        _stepWaypointSound.SetActive(true);
        await UniTask.Delay(TimeSpan.FromSeconds(_delayBeforeLightOff), ignoreTimeScale: false, cancellationToken: ct);
        _lightFlicker.gameObject.SetActive(false);
        _lightFlicker1.gameObject.SetActive(false);
        await UniTask.Delay(TimeSpan.FromSeconds(_delayBeforeDeactivateScene), ignoreTimeScale: false, cancellationToken: ct);
        gameObject.SetActive(false);

        EndOfHeadBeatScene?.Invoke();
    }
}