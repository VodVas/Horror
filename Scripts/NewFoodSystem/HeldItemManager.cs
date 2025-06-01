using UnityEngine;

public sealed class HeldItemManager : MonoBehaviour
{
    [SerializeField] private Transform _holdPosition;

    private Food _itemInHand;

    public bool HasItem => _itemInHand != null;
    public Food CurrentItem => _itemInHand;

    public bool TryTakeItem(Food item)
    {
        if (_itemInHand != null) return false;

        if (!item.CanInteract) return false;

        _itemInHand = item;
        ConfigureItemInHand(item);
        return true;
    }

    public Food ReleaseItem()
    {
        if (_itemInHand == null) return null;

        var item = _itemInHand;
        _itemInHand = null;

        item.transform.SetParent(null);
        return item;
    }

    private void ConfigureItemInHand(Food item)
    {
        item.transform.SetParent(_holdPosition);
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.identity;
        item.ChangeState(ItemState.InHand);
    }
}