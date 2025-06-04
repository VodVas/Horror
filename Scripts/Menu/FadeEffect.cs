using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class FadeEffect : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image _fadeImage;
    [SerializeField] private CanvasGroup _canvasGroup;

    [Header("Dependencies")]
    [SerializeField] private ScreamerActivator _screamerActivator;

    [Header("Fade Settings")]
    [SerializeField] private float _fadeDuration = 1f;
    [SerializeField] private Color _fadeColor = Color.black;
    [SerializeField] private AnimationCurve _fadeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private bool _useUnscaledTime = false;
    [SerializeField] private bool _deactivateWhenTransparent = true;

    private Tween _currentFadeTween;
    private bool _isInitialized;

    public bool IsFading => _currentFadeTween != null && _currentFadeTween.IsActive() && _currentFadeTween.IsPlaying();
    public float CurrentAlpha => _canvasGroup != null ? _canvasGroup.alpha : (_fadeImage != null ? _fadeImage.color.a : 0f);
    public event Action<float> OnFadeProgress;
    public event Action<bool> OnFadeComplete;

    private void Awake()
    {
        if (!ValidateReferences())
        {
            enabled = false;
            return;
        }

        InitializeFadeTarget();
        _isInitialized = true;
    }

    private bool ValidateReferences()
    {
        bool hasValidTarget = _fadeImage != null || _canvasGroup != null;

        if (!hasValidTarget)
        {
            Debug.LogError($"[{nameof(FadeEffect)}] Neither Image nor CanvasGroup is assigned!", this);
            return false;
        }

        if (_screamerActivator == null)
        {
            Debug.LogWarning($"[{nameof(FadeEffect)}] ScreamerActivator is not assigned. Auto-fade disabled.", this);
        }

        if (_canvasGroup == null && _fadeImage != null)
        {
            _canvasGroup = _fadeImage.GetComponent<CanvasGroup>();
            if (_canvasGroup == null)
            {
                _canvasGroup = _fadeImage.gameObject.AddComponent<CanvasGroup>();
            }
        }

        return true;
    }

    private void InitializeFadeTarget()
    {
        SetAlpha(0f);

        if (_deactivateWhenTransparent)
        {
            SetFadeObjectActive(false);
        }
    }

    private void OnEnable()
    {
        if (_screamerActivator != null)
        {
            _screamerActivator.OnSequenceCompleted += HandleSequenceCompleted;
        }
    }

    private void OnDisable()
    {
        if (_screamerActivator != null)
        {
            _screamerActivator.OnSequenceCompleted -= HandleSequenceCompleted;
        }
    }

    private void OnDestroy()
    {
        KillCurrentTween();
    }

    public void FadeIn(Action onComplete = null)
    {
        if (!_isInitialized) return;

        Fade(1f, onComplete);
    }

    public void FadeOut(Action onComplete = null)
    {
        if (!_isInitialized) return;

        Fade(0f, () =>
        {
            if (_deactivateWhenTransparent)
            {
                SetFadeObjectActive(false);
            }
            onComplete?.Invoke();
        });
    }

    public void FadeToAlpha(float targetAlpha, Action onComplete = null)
    {
        if (!_isInitialized) return;

        targetAlpha = Mathf.Clamp01(targetAlpha);
        Fade(targetAlpha, onComplete);
    }

    public void SetAlphaInstant(float alpha)
    {
        if (!_isInitialized) return;

        KillCurrentTween();
        SetAlpha(Mathf.Clamp01(alpha));

        if (_deactivateWhenTransparent && Mathf.Approximately(alpha, 0f))
        {
            SetFadeObjectActive(false);
        }
    }

    private void Fade(float targetAlpha, Action onComplete)
    {
        KillCurrentTween();

        if (targetAlpha > 0f || !_deactivateWhenTransparent)
        {
            SetFadeObjectActive(true);
        }

        float startAlpha = CurrentAlpha;

        _currentFadeTween = DOTween.To(
            () => startAlpha,
            x => SetAlpha(x),
            targetAlpha,
            _fadeDuration)
            .SetEase(_fadeCurve)
            .SetUpdate(_useUnscaledTime)
            .OnUpdate(() =>
            {
                OnFadeProgress?.Invoke(CurrentAlpha);
            })
            .OnComplete(() =>
            {
                bool fadedIn = targetAlpha > 0.5f;
                OnFadeComplete?.Invoke(fadedIn);
                onComplete?.Invoke();

                if (_deactivateWhenTransparent && Mathf.Approximately(targetAlpha, 0f))
                {
                    SetFadeObjectActive(false);
                }
            });
    }

    private void SetAlpha(float alpha)
    {
        if (_canvasGroup != null)
        {
            _canvasGroup.alpha = alpha;
        }
        else if (_fadeImage != null)
        {
            var color = _fadeImage.color;
            color.a = alpha;
            _fadeImage.color = color;
        }
    }

    private void SetFadeObjectActive(bool active)
    {
        GameObject target = null;

        if (_canvasGroup != null)
        {
            target = _canvasGroup.gameObject;
        }
        else if (_fadeImage != null)
        {
            target = _fadeImage.gameObject;
        }

        if (target != null && target.activeSelf != active)
        {
            target.SetActive(active);
        }
    }

    private void KillCurrentTween()
    {
        if (_currentFadeTween != null && _currentFadeTween.IsActive())
        {
            _currentFadeTween.Kill();
            _currentFadeTween = null;
        }
    }

    private void HandleSequenceCompleted()
    {
        FadeIn();
    }

    public void StopFade()
    {
        KillCurrentTween();
    }

    public void ToggleFade(Action onComplete = null)
    {
        float currentAlpha = CurrentAlpha;

        if (currentAlpha > 0.5f)
        {
            FadeOut(onComplete);
        }
        else
        {
            FadeIn(onComplete);
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        _fadeDuration = Mathf.Max(0.01f, _fadeDuration);

        if (_fadeImage != null && Application.isPlaying)
        {
            SetAlpha(CurrentAlpha);
        }
    }
#endif
}