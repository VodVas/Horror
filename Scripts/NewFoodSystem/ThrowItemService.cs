using UnityEngine;

public sealed class ThrowItemService : MonoBehaviour
{
    [SerializeField] private float _throwForce = 10f;

    public void Throw(GameObject item, Vector3 direction)
    {
        if (item.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.AddForce(direction * _throwForce, ForceMode.Impulse);
        }
    }
}