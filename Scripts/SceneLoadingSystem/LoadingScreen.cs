using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
public sealed class LoadingScreen : MonoBehaviour, ILoadingScreen
{
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private Image _progressBar;
    [SerializeField] private float _fadeDuration = 0.3f;
    [SerializeField] private AnimationCurve _fadeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private Canvas _canvas;
    private float _targetProgress;
    private float _currentProgress;

    private void Awake()
    {
        _canvas = GetComponent<Canvas>();
        _canvas.enabled = false;
        if (_canvasGroup == null)
            _canvasGroup = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();
        _canvasGroup.alpha = 0f;
    }

    private void Update()
    {
        if (Mathf.Approximately(_currentProgress, _targetProgress)) return;

        _currentProgress = Mathf.Lerp(_currentProgress, _targetProgress, Time.deltaTime * 5f);
        if (_progressBar != null)
            _progressBar.fillAmount = _currentProgress;
    }

    public async UniTask ShowAsync(CancellationToken cancellationToken)
    {
        _canvas.enabled = true;
        _currentProgress = 0f;
        _targetProgress = 0f;
        if (_progressBar != null)
            _progressBar.fillAmount = 0f;

        await FadeCanvasGroup(0f, 1f, cancellationToken);
    }

    public async UniTask HideAsync(CancellationToken cancellationToken)
    {
        await FadeCanvasGroup(1f, 0f, cancellationToken);
        _canvas.enabled = false;
    }

    public void UpdateProgress(float progress)
    {
        _targetProgress = Mathf.Clamp01(progress);
    }

    private async UniTask FadeCanvasGroup(float from, float to, CancellationToken cancellationToken)
    {
        var elapsed = 0f;
        while (elapsed < _fadeDuration)
        {
            cancellationToken.ThrowIfCancellationRequested();
            elapsed += Time.unscaledDeltaTime;
            var t = Mathf.Clamp01(elapsed / _fadeDuration);
            _canvasGroup.alpha = Mathf.Lerp(from, to, _fadeCurve.Evaluate(t));
            await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
        }
        _canvasGroup.alpha = to;
    }
}