using UnityEngine;

public interface IFeedbackAnimator
{
    void PlayAnimation(RectTransform element, CanvasGroup canvasGroup, Vector2 startPosition);
    void StopAnimation();
    bool IsAnimating { get; }
}
