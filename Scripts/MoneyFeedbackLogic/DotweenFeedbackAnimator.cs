using UnityEngine;
using DG.Tweening;

public sealed class DotweenFeedbackAnimator : IFeedbackAnimator
{
    private readonly FeedbackAnimationData _data;
    private Tweener _moveTween;
    private Tweener _fadeTween;
    private bool _isActive;

    public bool IsAnimating => _isActive;

    public DotweenFeedbackAnimator(FeedbackAnimationData data)
    {
        _data = data;
    }

    public void PlayAnimation(RectTransform element, CanvasGroup canvasGroup, Vector2 startPosition)
    {
        StopAnimation();
        ResetState(element, canvasGroup, startPosition);

        _isActive = true;

        _moveTween = element.DOAnchorPos(startPosition + _data.riseOffset, _data.riseDuration)
            .SetEase(_data.movementEase)
            .OnComplete(() => {
                _fadeTween = canvasGroup.DOFade(0f, _data.fadeDuration)
                    .OnComplete(() => _isActive = false);
            });
    }

    public void StopAnimation()
    {
        _moveTween?.Kill();
        _fadeTween?.Kill();
        _isActive = false;
    }

    private void ResetState(RectTransform element, CanvasGroup canvasGroup, Vector2 startPosition)
    {
        element.anchoredPosition = startPosition;
        canvasGroup.alpha = 1f;
    }
}