using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

[RequireComponent(typeof(Collider), typeof(AudioSource))]
public abstract class NPCInteractionHandler : MonoBehaviour
{
    private enum LipSyncMode { Precise, Hybrid }

    [Header("UI Feedback")]
    [SerializeField] private RectTransform _feedbackElement;
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private FeedbackAnimationData _animationData;

    [Header("Audio")]
    [SerializeField] private AudioSource _audioSource;

    [Header("Lip Sync Settings")]
    [SerializeField] private LipSyncMode _lipSyncMode = LipSyncMode.Hybrid;
    [SerializeField] protected PreciseLipSyncController _preciseLipSyncController;
    [SerializeField] protected HybridLipSync _hybridLipSyncController;

    [Header("Precise Mode Clips")]
    [SerializeField] protected PhonemicClip[] _preciseDialogueClips;

    [Header("Hybrid Mode Clips")]
    [SerializeField] protected AudioClip[] _hybridDialogueClips;

    private Vector2 _initialPosition;
    private IFeedbackAnimator _animator;
    private CancellationTokenSource _interactionCts;
    private bool _isInteracting;

    protected virtual void Awake()
    {
        _initialPosition = _feedbackElement.anchoredPosition;
        _canvasGroup.alpha = 0f;
        _animator = new DotweenFeedbackAnimator(_animationData);

        if (_lipSyncMode == LipSyncMode.Precise && _preciseLipSyncController == null)
            Debug.LogError("PreciseLipSyncController is not assigned!", this);
        if (_lipSyncMode == LipSyncMode.Hybrid && _hybridLipSyncController == null)
            Debug.LogError("HybridLipSyncController is not assigned!", this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!CanInteract(other)) return;

        HandleInteraction(other).Forget();
    }

    private void OnDestroy()
    {
        if (this == null || gameObject == null) return;

        CancelCurrentInteraction();
    }

    private bool CanInteract(Collider other)
    {
        return other.TryGetComponent(out IInteractable _)
            && !_isInteracting
            && !_animator.IsAnimating;
    }

    private async UniTaskVoid HandleInteraction(Collider other)
    {
        _isInteracting = true;
        CancelCurrentInteraction();
        _interactionCts = new CancellationTokenSource();

        try
        {
            bool shouldGiveReward = CheckIfRewardable(other);

            if (shouldGiveReward)
            {
                PlayMoneyEffects();

                await UniTask.WhenAll(
                    PlayFeedbackAnimation(_interactionCts.Token),
                    PlayDialogue(_interactionCts.Token)
                );
            }


            ReturnFoodToPool(other);
        }
        finally
        {
            _isInteracting = false;
        }
    }

    private bool CheckIfRewardable(Collider other)
    {
        if (other.TryGetComponent(out IRewardableFood rewardableFood))
        {
            return rewardableFood.IsRewardable();
        }

        if (other.TryGetComponent(out IValidatable validatable))
        {
            return validatable.IsValidProduct();
        }

        return false;
    }

    private void ReturnFoodToPool(Collider other)
    {
        if (other.TryGetComponent(out Food food))
        {
            food.ReturnToPool();
        }
    }

    private void PlayMoneyEffects()
    {
        _audioSource.Play();
        _animator.PlayAnimation(_feedbackElement, _canvasGroup, _initialPosition);
    }

    private async UniTask PlayFeedbackAnimation(CancellationToken ct)
    {
        while (_animator.IsAnimating && !ct.IsCancellationRequested)
        {
            await UniTask.Yield(ct);
        }
    }

    private async UniTask PlayDialogue(CancellationToken ct)
    {
        switch (_lipSyncMode)
        {
            case LipSyncMode.Precise:
                await PlayPreciseDialogue(ct);
                break;
            case LipSyncMode.Hybrid:
                await PlayHybridDialogue(ct);
                break;
        }
    }

    private async UniTask PlayPreciseDialogue(CancellationToken ct)
    {
        if (_preciseLipSyncController == null || _preciseDialogueClips.Length == 0) return;

        var clipIndex = Random.Range(0, _preciseDialogueClips.Length);
        _preciseLipSyncController.PlayDialogue(_preciseDialogueClips[clipIndex].name);

        while (_preciseLipSyncController.IsPlaying && !ct.IsCancellationRequested)
        {
            await UniTask.Yield(ct);
        }
    }

    private async UniTask PlayHybridDialogue(CancellationToken ct)
    {
        if (_hybridLipSyncController == null || _hybridDialogueClips.Length == 0) return;

        var clip = _hybridDialogueClips[Random.Range(0, _hybridDialogueClips.Length)];
        _hybridLipSyncController.StartLipSync(clip);

        while (_hybridLipSyncController.IsPlaying && !ct.IsCancellationRequested)
        {
            await UniTask.Yield(ct);
        }
    }

    private void CancelCurrentInteraction()
    {
        _interactionCts?.Cancel();
        _interactionCts?.Dispose();
        _interactionCts = null;

        if (this == null || gameObject == null) return;
        if (!this || !gameObject) return;

        switch (_lipSyncMode)
        {
            case LipSyncMode.Precise:
                _preciseLipSyncController?.StopDialogue();
                break;
            case LipSyncMode.Hybrid:
                _hybridLipSyncController?.StopLipSync();
                break;
        }
    }
}
