using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using TMPro;

public sealed class AchievementDisplay : MonoBehaviour
{
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private TextMeshProUGUI _header;
    [SerializeField] private TextMeshProUGUI _description;

    public async UniTask ShowAchievement(
        string title,
        string description,
        float displayDuration,
        float fadeDuration,
        CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        _header.text = title;
        _description.text = description;

        await FadeAnimation(1f, fadeDuration, ct);
        await UniTask.Delay((int)(displayDuration * 1000), cancellationToken: ct);
        await FadeAnimation(0f, fadeDuration, ct);
    }

    private async UniTask FadeAnimation(float targetAlpha, float duration, CancellationToken ct)
    {
        float startAlpha = _canvasGroup.alpha;
        float elapsed = 0f;

        while (elapsed < duration && !ct.IsCancellationRequested)
        {
            _canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / duration);
            elapsed += Time.deltaTime;
            await UniTask.Yield(ct);
        }

        if (!ct.IsCancellationRequested)
        {
            _canvasGroup.alpha = targetAlpha;
        }
    }
}