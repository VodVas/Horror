using UnityEngine;

public interface IInventory
{
    bool HasItem { get; }
    IItem CurrentItem { get; }
    bool TryTakeItem(IItem item);
    void ThrowItem(Vector3 force);
    void ClearItem();
}
