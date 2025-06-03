using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(AudioSource))]
public class RoundSoundPlayer : MonoBehaviour
{
    [SerializeField] private AudioClip _musicClip;
    [SerializeField] private Transform _centerPoint;
    [SerializeField] private Vector3 _rotationAxis = Vector3.up;
    [SerializeField] private float _angularSpeed = 90f;
    [SerializeField] private float _spiralDuration = 10f;
    [SerializeField] private float _maxSpiralRadius = 5f;
    [SerializeField] private float _maxHeight = 10f;

    private AudioSource _audioSource;
    private float _currentAngle = 0f;
    private float _currentRadius;
    private float _currentHeight;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioSource.clip = _musicClip;
        _audioSource.playOnAwake = false;
    }

    private void Start()
    {
        if (_musicClip != null)
            _audioSource.Play();

        _currentRadius = _maxSpiralRadius;
        _currentHeight = _maxHeight;

        DOTween.To(() => _currentRadius, x => _currentRadius = x, 0f, _spiralDuration)
            .SetEase(Ease.Linear);

        DOTween.To(() => _currentHeight, y => _currentHeight = y, 0f, _spiralDuration)
            .SetEase(Ease.Linear);
    }

    private void Update()
    {
        _currentAngle += _angularSpeed * Time.deltaTime;

        Vector3 offset = Quaternion.AngleAxis(_currentAngle, _rotationAxis) * Vector3.forward * _currentRadius;
        transform.position = _centerPoint.position + offset + _rotationAxis * _currentHeight;
    }

    private void OnValidate()
    {
        _rotationAxis = _rotationAxis.normalized;
    }
}