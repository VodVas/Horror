using System.Collections;
using UnityEngine;

public class ObjectOnceShaker : MonoBehaviour
{
    [SerializeField] private float _duration = 0.5f;
    [SerializeField] private float _amplitude = 0.1f;

    private Transform _transform;
    private Vector3 _originalPosition;
    private Coroutine _shakeCoroutine;
    private bool _isShaking;

    private void Awake()
    {
        _transform = GetComponent<Transform>();

        if (_transform == null)
        {
            Debug.LogError("Missing Transform component!", this);
            enabled = false;
            return;
        }
    }

    [ContextMenu("Shake")]
    public void Shake()
    {
        if (_isShaking || _transform == null)
        {
            return;
        }

        if (_shakeCoroutine != null)
        {
            StopCoroutine(_shakeCoroutine);
        }

        _shakeCoroutine = StartCoroutine(ShakeRoutine());
    }

    private IEnumerator ShakeRoutine()
    {
        _isShaking = true;
        _originalPosition = _transform.localPosition;
        float elapsedTime = 0f;

        while (elapsedTime < _duration)
        {
            elapsedTime += Time.deltaTime;
            float normalizedTime = Mathf.Clamp01(elapsedTime / _duration);
            float currentAmplitude = _amplitude * (1f - normalizedTime);

            Vector3 randomOffset = new Vector3(
                Random.Range(-1f, 1f),
                Random.Range(-1f, 1f),
                0f
            ) * currentAmplitude;

            _transform.localPosition = _originalPosition + randomOffset;
            yield return null;
        }

        FinalizeShake();
    }

    private void FinalizeShake()
    {
        if (_transform != null)
        {
            _transform.localPosition = _originalPosition;
        }
        _isShaking = false;
        _shakeCoroutine = null;
    }

    private void OnDisable()
    {
        if (_isShaking)
        {
            FinalizeShake();
        }
    }
}