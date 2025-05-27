using UnityEngine;

public interface IItem
{
    ItemType Type { get; }
    ItemState State { get; }
    GameObject GameObject { get; }
    void ChangeState(ItemState newState);
    bool CanInteract();
    void OnInteract();
}
