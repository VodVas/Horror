using UnityEngine;

public class VerticalPingPongMover : MonoBehaviour
{
    private const float HeightThreshold = 0.01f;

    [Header("Movement Settings")]
    [SerializeField] private float _speed = 1f;
    [SerializeField] private float _range = 2f;
    [SerializeField] private bool _isActive = true;

    [Header("Sound Settings")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _landingSound;

    private Vector3 _startPosition;
    private float _currentHeight;
    private bool _isMovingUp = true;
    private bool _wasInUpperPosition = false;

    private void Start()
    {
        if (_landingSound == null || _audioSource == null)
        {
            Debug.Log("Audio not assigned", this);
            enabled = false;
            return;
        }

        _startPosition = transform.position;
        _audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (!_isActive) return;

        Move();
        CheckHeightLimit();
    }

    public void StartMovement()
    {
        _isActive = true;
    }

    public void StopAndReset()
    {
        _isActive = false;
        transform.position = _startPosition;
        _currentHeight = 0f;
    }

    public void SetSpeed(float speed)
    {
        _speed = speed;
    }

    public void SetRange(float range)
    {
        _range = range;
    }

    private void Move()
    {
        float direction = _isMovingUp ? 1f : -1f;
        _currentHeight += direction * _speed * Time.deltaTime;
        _currentHeight = Mathf.Clamp(_currentHeight, 0f, _range);

        transform.position = _startPosition + Vector3.up * _currentHeight;
    }

    private void CheckHeightLimit()
    {
        if (_isMovingUp)
        {
            if (_currentHeight < _range - HeightThreshold) return;

            _wasInUpperPosition = true;
        }
        else
        {
            if (_currentHeight > HeightThreshold) return;

            if (_wasInUpperPosition && _landingSound != null)
            {
                _audioSource.PlayOneShot(_landingSound);

                _wasInUpperPosition = false;
            }
        }

        _isMovingUp = !_isMovingUp;
    }
}