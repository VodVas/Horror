using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using LiquidVolumeFX;

[RequireComponent(typeof(LiquidVolume))]
public class LiquidVolumeAnimator : MonoBehaviour
{
    private LiquidVolume _liquidVolume;
    private CancellationTokenSource _animationCts;

    private void Awake()
    {
        _liquidVolume = GetComponent<LiquidVolume>();
    }

    private void OnDestroy()
    {
        CancelActiveAnimation();
    }

    public void AnimateFill(float targetLevel, float duration)
    {
        CancelActiveAnimation();
        _animationCts = new CancellationTokenSource();

        RunFillAnimation(targetLevel, duration, _animationCts.Token).Forget();
    }

    private async UniTaskVoid RunFillAnimation(
        float targetLevel,
        float duration,
        CancellationToken cancellationToken)
    {
        float startLevel = _liquidVolume.level;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / duration);
            _liquidVolume.level = Mathf.Lerp(startLevel, targetLevel, progress);

            await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
        }

        _liquidVolume.level = targetLevel;
    }

    private void CancelActiveAnimation()
    {
        if (_animationCts == null) return;

        _animationCts.Cancel();
        _animationCts.Dispose();
        _animationCts = null;
    }
}