using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(SphereCollider))]
public sealed class InteractionProjectile : MonoBehaviour, IInteractionProjectile
{
    [SerializeField] private float MaxLifetime = 0.5f;

    private Rigidbody _rigidbody;
    private SphereCollider _collider;
    private float _lifetime;

    private Action<IInteractable> _onHit;
    private Action<InteractionProjectile> _onReturn;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<SphereCollider>();
        _rigidbody.useGravity = false;
        _rigidbody.isKinematic = false;
        _collider.isTrigger = true;
        gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
    }

    private void Update()
    {
        _lifetime -= Time.deltaTime;

        if (_lifetime <= 0)
        {
            _onHit?.Invoke(null);
            ReturnToPool();
        }
    }

    public void Initialize(Action<InteractionProjectile> onReturn)
    {
        _onReturn = onReturn;
    }

    public void Launch(Vector3 origin, Vector3 direction, float speed, Action<IInteractable> onHit)
    {
        transform.position = origin;
        _rigidbody.velocity = direction * speed;
        _onHit = onHit;
        _lifetime = MaxLifetime;
        gameObject.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IInteractable interactable) && interactable.CanInteract)
        {
            _onHit?.Invoke(interactable);
            ReturnToPool();
        }
    }

    private void ReturnToPool()
    {
        gameObject.SetActive(false);
        _rigidbody.velocity = Vector3.zero;
        _onHit = null;
        _onReturn?.Invoke(this);
    }
}