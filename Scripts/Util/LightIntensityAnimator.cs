using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class LightIntensityAnimator : MonoBehaviour
{
    [SerializeField] private bool _isFlicking = false;

    [Header("Intensity Animation")]
    [SerializeField] private float _targetIntensity;
    [SerializeField] private float _duration;

    [Header("Flicker Settings")]
    [SerializeField] private bool _enableFlicker;
    [SerializeField, Min(0.01f)] private float _flickerMinInterval = 0.1f;
    [SerializeField, Min(0.01f)] private float _flickerMaxInterval = 0.5f;
    [SerializeField, Range(0f, 1f)] private float _flickerIntensityDrop = 0.7f;

    private Light _pointLight;
    private CancellationTokenSource _cts;
    private float _baseIntensity;
    private bool _isFlickering;

    private void Awake()
    {
        _pointLight = GetComponent<Light>();
        _cts = new CancellationTokenSource();
        _baseIntensity = _pointLight.intensity;

        if (!_isFlicking) return;

        StartIntensityAnimation();
    }

    private void OnDestroy()
    {
        _cts?.Cancel();
        _cts?.Dispose();
    }

    [ContextMenu("StartIntensityAnimation")]
    public void StartIntensityAnimation()
    {
        AnimateIntensityAsync(_targetIntensity, _duration, _cts.Token).Forget();
        if (_enableFlicker) StartFlicker().Forget();
    }

    public void StopAllEffects()
    {
        _cts?.Cancel();
        _cts?.Dispose();
        _cts = new CancellationTokenSource();
        _pointLight.intensity = _baseIntensity;
    }

    private async UniTaskVoid AnimateIntensityAsync(float targetIntensity, float duration, CancellationToken ct)
    {
        float startIntensity = _pointLight.intensity;
        float elapsed = 0f;

        while (elapsed < duration && !ct.IsCancellationRequested)
        {
            float progress = elapsed / duration;
            float target = Mathf.Lerp(startIntensity, targetIntensity, progress);

            if (!_isFlickering)
                _pointLight.intensity = target;

            elapsed += Time.deltaTime;
            await UniTask.Yield(PlayerLoopTiming.Update, ct);
        }

        if (!ct.IsCancellationRequested)
            _pointLight.intensity = targetIntensity;
    }

    private async UniTaskVoid StartFlicker()
    {
        if (_isFlickering) return;
        _isFlickering = true;

        try
        {
            while (!_cts.IsCancellationRequested)
            {
                float currentTarget = Mathf.Lerp(_baseIntensity, _targetIntensity,
                    Mathf.Clamp01(Time.time / _duration));

                _pointLight.intensity = currentTarget * _flickerIntensityDrop;
                await UniTask.Delay(
                    TimeSpan.FromSeconds(UnityEngine.Random.Range(_flickerMinInterval, _flickerMaxInterval)),
                    cancellationToken: _cts.Token);

                _pointLight.intensity = currentTarget;
                await UniTask.Delay(
                    TimeSpan.FromSeconds(UnityEngine.Random.Range(_flickerMinInterval, _flickerMaxInterval)),
                    cancellationToken: _cts.Token);
            }
        }
        finally
        {
            _isFlickering = false;
        }
    }
}