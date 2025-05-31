using System;
using UnityEngine;

public class ItemSpawnPoint : MonoBehaviour, IInteractable
{
    [field: SerializeField] public ItemType itemType { get; private set; }

    public event Action<ItemType, Vector3> OnSpawnRequested;

    public bool CanInteract => true;

    public void Interact()
    {
        OnSpawnRequested?.Invoke(itemType, transform.position);
    }
}
