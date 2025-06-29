using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public sealed class AxisForceApplier : MonoBehaviour
{
    [SerializeField] private float _forceMagnitude = 10f;
    [SerializeField] private Vector3 _forceDirection = Vector3.forward;
    [SerializeField] private ForceMode _forceMode = ForceMode.Impulse;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _audioClip;

    private Rigidbody _rigidbody;

    private void Awake()
    {
        if (_audioSource == null || _audioClip == null)
        {
            Debug.Log("Audio not assigned", this);
            enabled = false;
            return;
        }

        _rigidbody = GetComponent<Rigidbody>();
    }

    [ContextMenu("ApplyForce")]
    public void ApplyForce()
    {
        Vector3 force = _forceDirection.normalized * _forceMagnitude;
        _rigidbody.AddForce(force, _forceMode);
        _audioSource.PlayOneShot(_audioClip);
    }
}