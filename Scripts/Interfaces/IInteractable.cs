//public interface IInteractable
//{
//    bool CanInteract { get; }
//    void Interact();
//}

using UnityEngine;

public interface IInteractable
{
    bool CanInteract { get; }
    Transform Transform { get; }
    void Interact();
}