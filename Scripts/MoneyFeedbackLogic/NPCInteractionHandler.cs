using UnityEngine;
using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;

[RequireComponent(typeof(Collider), typeof(AudioSource))]
public abstract class NPCInteractionHandler : MonoBehaviour
{
    [Header("Base Dependencies")]
    [SerializeField] private RectTransform _feedbackElement;
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private AudioSource _moneySound;
    [SerializeField] private FacialExpressionController _facialExpression;

    [Header("Base Animation Configuration")]
    [SerializeField] private FeedbackAnimationData _animationData;

    private Vector2 _initialPosition;
    private IFeedbackAnimator _animator;
    private CancellationTokenSource _resetEmotionCts;

    private void Awake()
    {
        _initialPosition = _feedbackElement.anchoredPosition;
        _canvasGroup.alpha = 0f;
        _animator = new DotweenFeedbackAnimator(_animationData);

        AdditionalAwakeInitialization();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out IPickupable _) || _animator.IsAnimating) return;

        StartCoroutine(InteractionRoutine());

        if (other.TryGetComponent(out Food food))
        {
            food.ReturnToPool();
        }
    }

    private void OnDestroy()
    {
        _resetEmotionCts?.Cancel();
        _resetEmotionCts?.Dispose();
        _animator.StopAnimation();
    }

    protected void PlayInteraction(FacialEmotionPreset preset)
    {
        PlayEmotion(preset);
    }

    protected abstract void PlaySpecificSounds();

    private void PlayEmotion(FacialEmotionPreset preset)
    {
        if (_facialExpression == null || preset == null) return;

        var emotion = preset.GetRandomNonNeutralEmotion();
        if (emotion == null) return;

        _resetEmotionCts?.Cancel();
        _resetEmotionCts?.Dispose();

        _resetEmotionCts = new CancellationTokenSource();
        ResetEmotionAfterDelay(emotion.DefaultDuration).Forget();

        _facialExpression.SetEmotion(emotion.Type, emotion.DefaultDuration);
    }

    private async UniTaskVoid ResetEmotionAfterDelay(float delay)
    {
        try
        {
            await UniTask.Delay((int)(delay * 1000),
                cancellationToken: _resetEmotionCts.Token);

            _facialExpression.ResetToNeutral();
        }
        finally
        {
            _resetEmotionCts?.Dispose();
            _resetEmotionCts = null;
        }
    }

    protected virtual void AdditionalAwakeInitialization() { }
    protected abstract void HandleInteraction();

    private IEnumerator InteractionRoutine()
    {
        _moneySound.Play();
        PlaySpecificSounds();
        HandleInteraction();
        _animator.PlayAnimation(_feedbackElement, _canvasGroup, _initialPosition);

        while (_animator.IsAnimating)
        {
            yield return null;
        }
    }
}