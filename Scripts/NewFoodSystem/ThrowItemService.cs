using UnityEngine;

public sealed class ThrowItemService
{
    private readonly float _throwForce;

    public ThrowItemService(float throwForce)
    {
        _throwForce = throwForce;
    }

    public void Throw(GameObject item, Vector3 direction)
    {
        if (item == null) return;

        if (item.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.isKinematic = false;
            rb.AddForce(direction * _throwForce, ForceMode.Impulse);
        }
    }
}

