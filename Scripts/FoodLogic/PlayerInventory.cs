using UnityEngine;

public class PlayerInventory : MonoBehaviour, IInventory
{
    [SerializeField] private Transform handTransform;
    [SerializeField] private float throwForce = 10f;

    private IItem currentItem;

    public bool HasItem => currentItem != null;
    public IItem CurrentItem => currentItem;

    public bool TryTakeItem(IItem item)
    {
        if (HasItem || !item.CanInteract()) return false;

        currentItem = item;
        item.GameObject.transform.SetParent(handTransform);
        item.GameObject.transform.localPosition = Vector3.zero;
        item.ChangeState(ItemState.InHand);

        return true;
    }

    public void ThrowItem(Vector3 direction)
    {
        if (!HasItem) return;

        var rb = currentItem.GameObject.GetComponent<Rigidbody>();
        if (!rb) rb = currentItem.GameObject.AddComponent<Rigidbody>();

        currentItem.GameObject.transform.SetParent(null);
        currentItem.ChangeState(ItemState.Thrown);
        rb.AddForce(direction * throwForce, ForceMode.Impulse);

        currentItem = null;
    }

    public void ClearItem()
    {
        if (HasItem)
        {
            currentItem.GameObject.transform.SetParent(null);
            currentItem = null;
        }
    }
}
