using System;
using UnityEngine;

public class ItemSpawnPoint : MonoBehaviour, IInteractable
{
    [field: SerializeField] public ItemType itemType { get; private set; }

    public event Action<ItemType> OnSpawnRequested;

    public void Interact()
    {
        OnSpawnRequested?.Invoke(itemType);
    }
}