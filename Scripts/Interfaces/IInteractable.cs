using UnityEngine;

public interface IInteractable
{
    bool CanInteract { get; }
    Transform Transform { get; }
    void Interact();
}