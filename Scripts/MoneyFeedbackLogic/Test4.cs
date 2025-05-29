using DG.Tweening;
using UnityEngine;

public class Test4 { }
//{
//    using UnityEngine;
//using System.Collections;
//using DG.Tweening;

//[RequireComponent(typeof(Collider))]
//[RequireComponent(typeof(AudioSource))]
//public sealed class NPCInteractionHandler : MonoBehaviour
//{
//    [Header("UI References")]
//    [SerializeField] private RectTransform _feedbackElement;
//    [SerializeField] private CanvasGroup _canvasGroup;

//    [Header("Animation Settings")]
//    [SerializeField] private float _riseDuration = 0.8f;
//    [SerializeField] private float _fadeDuration = 0.3f;
//    [SerializeField] private Vector2 _riseOffset = new(0, 80f);
//    [SerializeField] private Ease _movementEase = Ease.OutSine;

//    private AudioSource _moneySound;
//    private Vector2 _initialPosition;
//    private Coroutine _animationRoutine;
//    private WaitForSecondsRealtime _fadeDelayWait;

//    private void Awake()
//    {
//        _fadeDelayWait = new WaitForSecondsRealtime(0.4f);
//        _moneySound = GetComponent<AudioSource>();
//        _initialPosition = _feedbackElement.anchoredPosition;
//        _canvasGroup.alpha = 0f;
//    }

//    private void OnTriggerEnter(Collider other)
//    {
//        if (!other.TryGetComponent(out IPickupable _) || _animationRoutine != null) return;

//        _animationRoutine = StartCoroutine(FeedbackAnimationRoutine());
//    }

//    private void OnDestroy()
//    {
//        if (_animationRoutine != null)
//        {
//            StopCoroutine(_animationRoutine);
//            DOTween.Kill(_feedbackElement);
//            DOTween.Kill(_canvasGroup);
//        }
//    }

//    private IEnumerator FeedbackAnimationRoutine()
//    {
//        ResetUIState();

//        bool interrupted = false;

//        _moneySound.Play();

//        var moveTween = _feedbackElement.DOAnchorPos(_initialPosition + _riseOffset, _riseDuration)
//            .SetEase(_movementEase)
//            .OnKill(() => interrupted = true);

//        yield return _fadeDelayWait;

//        if (!interrupted)
//        {
//            var fadeTween = _canvasGroup.DOFade(0f, _fadeDuration);

//            yield return fadeTween.WaitForCompletion();
//        }

//        if (!interrupted)
//        {
//            _feedbackElement.anchoredPosition = _initialPosition;
//        }

//        _animationRoutine = null;
//    }

//    private void ResetUIState()
//    {
//        if (_animationRoutine != null)
//        {
//            StopCoroutine(_animationRoutine);
//            DOTween.Kill(_feedbackElement);
//            DOTween.Kill(_canvasGroup);
//        }

//        _feedbackElement.anchoredPosition = _initialPosition;
//        _canvasGroup.alpha = 1f;
//    }
//}