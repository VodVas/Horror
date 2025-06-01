using System;
using UnityEngine;

public abstract class Food : MonoBehaviour, ITerminatable, IItemStateManager, IInteractable
{
    private const string Default = "Default";
    private const string Weapon = "Weapon";
    private const string Interactable = "Interactable";

    [SerializeField] private ItemState _currentState = ItemState.InPool;

    private Rigidbody _rigidbody;
    private Collider _collider;

    public event Action<ITerminatable> Terminated;
    public event Action<Food> OnInteractionRequested;

    public abstract ItemType ItemType { get; }
    public ItemState CurrentState => _currentState;
    public virtual bool CanInteract => _currentState == ItemState.Available || _currentState == ItemState.Thrown;

    public Transform Transform => GetComponent<Transform>();

    protected virtual void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
    }

    public void ChangeState(ItemState newState)
    {
        if (_currentState == newState) return;

        _currentState = newState;
        ConfigureForState(newState);
    }

    public void Interact()
    {
        if (!CanInteract) return;
        OnInteractionRequested?.Invoke(this);
    }

    public void ResetForPool()
    {
        ChangeState(ItemState.InPool);
        transform.SetParent(null);
        StopAllCoroutines();
    }

    public void ReturnToPool()
    {
        Terminated?.Invoke(this);
    }

    private void ConfigureForState(ItemState state)
    {
        switch (state)
        {
            case ItemState.InPool:
                ConfigureInPool();
                break;
            case ItemState.Available:
                ConfigureAvailable();
                break;
            case ItemState.InHand:
                ConfigureInHand();
                break;
            case ItemState.Thrown:
                ConfigureThrown();
                break;
        }
    }

    private void ConfigureInPool()
    {
        gameObject.SetActive(false);
        if (_rigidbody) _rigidbody.isKinematic = true;
        if (_collider) _collider.enabled = true;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    private void ConfigureAvailable()
    {
        gameObject.SetActive(true);
        gameObject.layer = LayerMask.NameToLayer(Default);
        if (_rigidbody)
        {
            _rigidbody.isKinematic = false;
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
        }
        if (_collider) _collider.enabled = true;
    }

    private void ConfigureInHand()
    {
        gameObject.layer = LayerMask.NameToLayer(Weapon);
        if (_rigidbody) _rigidbody.isKinematic = true;
        if (_collider) _collider.enabled = false;
        OnConfigureInHand();
    }

    private void ConfigureThrown()
    {
        gameObject.layer = LayerMask.NameToLayer(Interactable);
        if (_rigidbody) _rigidbody.isKinematic = false;
        if (_collider) _collider.enabled = true;
    }

    protected virtual void OnConfigureInHand() { }
}