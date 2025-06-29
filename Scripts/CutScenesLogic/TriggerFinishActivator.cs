using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class TriggerFinishActivator : MonoBehaviour
{
    [SerializeField] private FadeEffect _fadeEffect;
    [SerializeField] private GameObject _surviveMessage;
    [SerializeField] private GameObject _aboutGameMessage;
    [SerializeField] private GameObject _dave;
    [SerializeField] private GameObject _radio;
    [SerializeField] private ScreamerActivator _screamerActivator;
    [SerializeField] private GameObject _kilimangaroMusic;
    [SerializeField] private float _delay = 2f;

    private void Awake()
    {
        if (_fadeEffect == null || _surviveMessage == null || _aboutGameMessage == null || _screamerActivator == null || _kilimangaroMusic == null || _dave == null || _radio ==null)
        {
            Debug.Log("FadeEffect not assigned", this);
            enabled = false;
            return;
        }
    }

    private void OnEnable()
    {
        _screamerActivator.OnSequenceCompleted += ShowFinishDisplay;
    }

    private void OnDisable()
    {
        _screamerActivator.OnSequenceCompleted -= ShowFinishDisplay;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Player _))
        {
            Time.timeScale = 0f;
            DelayShowMessage().Forget();
            _fadeEffect.FadeIn();
        }
    }

    public void ShowFinishDisplay()
    {
        _dave.SetActive(false);
        _radio.SetActive(false);
        Time.timeScale = 0f;
        _fadeEffect.FadeIn();

        DelayShowAboutGameMessage().Forget();
    }

    private async UniTaskVoid DelayShowAboutGameMessage()
    {
        try
        {
            var ct = this.GetCancellationTokenOnDestroy();
            await UniTask.Delay(TimeSpan.FromSeconds(_delay), ignoreTimeScale: true, cancellationToken: ct);

            _aboutGameMessage.SetActive(true);
            _kilimangaroMusic.SetActive(true);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
    }

    private async UniTaskVoid DelayShowMessage()
    {
        try
        {
            var ct = this.GetCancellationTokenOnDestroy();
            await UniTask.Delay(TimeSpan.FromSeconds(_delay), ignoreTimeScale: true, cancellationToken: ct);

            _dave.SetActive(false);
            _surviveMessage.SetActive(true);
            _kilimangaroMusic.SetActive(true);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
    }
}