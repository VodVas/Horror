//using UnityEngine;

//public sealed class ItemManager : MonoBehaviour, IItemManager
//{
//    [SerializeField] private Transform _itemHoldPosition;
//    [SerializeField] private float _throwForce = 10f;

//    private GameObject _heldItem;
//    private const string WeaponLayer = "Weapon";
//    private const string DefaultLayer = "Default";

//    public bool HasItem => _heldItem != null;

//    public void TakeItem(GameObject item)
//    {
//        if (HasItem) return;

//        _heldItem = item;
//        ConfigureHeldItem();
//    }

//    public void ThrowItem()
//    {
//        if (!HasItem) return;

//        var item = ReleaseItem();
//        if (item.TryGetComponent<Rigidbody>(out var rb))
//        {
//            rb.isKinematic = false;
//            rb.AddForce(transform.forward * _throwForce, ForceMode.Impulse);
//        }
//    }

//    private void ConfigureHeldItem()
//    {
//        _heldItem.layer = LayerMask.NameToLayer(WeaponLayer);
//        _heldItem.transform.SetParent(_itemHoldPosition, false);
//        _heldItem.transform.localPosition = Vector3.zero;
//        _heldItem.transform.localRotation = Quaternion.identity;

//        if (_heldItem.TryGetComponent<Rigidbody>(out var rb))
//            rb.isKinematic = true;

//        if (_heldItem.TryGetComponent<Drink>(out _))
//            _heldItem.transform.localRotation *= Quaternion.Euler(90f, 0, 0);
//    }

//    private GameObject ReleaseItem()
//    {
//        var item = _heldItem;
//        item.layer = LayerMask.NameToLayer(DefaultLayer);
//        item.transform.SetParent(null);
//        _heldItem = null;
//        return item;
//    }
//}
