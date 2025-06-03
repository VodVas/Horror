using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SecondCutsceneActivator : MonoBehaviour
{
    private const string IsRunning = "IsRunning";
    private const string IsAgony = "IsAgony";
    private const string IsCrawl = "IsCrawl";

    [SerializeField] private GameObject _light;
    [SerializeField] private Animator _animator;
    [SerializeField] private AudioClip _scream;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private EmoSetter _emoSetter;
    [SerializeField] private GameObject _bodyCollider;
    [SerializeField] private GameObject _headCollider;

    [Header("Third cutscene")]
    [SerializeField] private float _delayBefore3Cutscene = 5f;
    [SerializeField] private float _thirdCutsceneDuration = 5f;
    [SerializeField] private GameObject _thirdTimeLine;
    [SerializeField] private GameObject _canvas;
    [SerializeField] private WaypointMover _waypointMover;
    [SerializeField] private GameObject _dave;

    public event Action EndOf2Scene;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Dave _))
        {
            _emoSetter.SetEnterEmotion();
            _audioSource.PlayOneShot(_scream);
            _light.SetActive(false);
            _animator.SetBool(IsRunning, false);
            _animator.SetBool(IsAgony, true);

            if (_bodyCollider.TryGetComponent(out BoxCollider collider))
            {
                collider.enabled = false;
            }

            if (_headCollider.TryGetComponent(out BoxCollider colliderHead))
            {
                colliderHead.enabled = false;
            }

            StartCoroutine(DelayingThirdCutscene());
        }
    }

    private IEnumerator DelayingThirdCutscene()
    {
        yield return new WaitForSeconds(_delayBefore3Cutscene);

        _canvas.SetActive(false);
        _thirdTimeLine.SetActive(true);
        _animator.SetBool(IsAgony, false);
        _animator.SetBool(IsCrawl, true);

        yield return new WaitForSeconds(0.5f);

        _waypointMover.enabled = true;

        yield return new WaitForSeconds(_thirdCutsceneDuration);

        _dave.SetActive(false);
        _thirdTimeLine.SetActive(false);
        _canvas.SetActive(true);
        EndOf2Scene?.Invoke();
    }
}