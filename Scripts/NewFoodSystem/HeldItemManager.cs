using UnityEngine;

public sealed class HeldItemManager
{
    private GameObject _itemInHand;
    private readonly Transform _holdPosition;
    private const string WeaponLayer = "Weapon";
    private const string DefaultLayer = "Default";

    public bool HasItem => _itemInHand != null;

    public HeldItemManager(Transform holdPosition)
    {
        _holdPosition = holdPosition;
    }

    public void TakeItem(GameObject item)
    {
        _itemInHand = item;

        ConfigureItemInHand();
    }

    private void ConfigureItemInHand()
    {
        _itemInHand.layer = LayerMask.NameToLayer(WeaponLayer);
        _itemInHand.transform.SetParent(_holdPosition, false);
        _itemInHand.transform.localPosition = Vector3.zero;
        _itemInHand.transform.localRotation = Quaternion.identity;

        if (_itemInHand.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.isKinematic = true;
        }

        if (_itemInHand.TryGetComponent<Drink>(out _))
        {
            _itemInHand.transform.localRotation *= Quaternion.Euler(90f, 0, 0);
        }
    }

    public GameObject ReleaseItem()
    {
        if (_itemInHand == null) return null;

        var item = _itemInHand;
        _itemInHand.layer = LayerMask.NameToLayer(DefaultLayer);
        _itemInHand.transform.SetParent(null);
        _itemInHand = null;
        return item;
    }
}