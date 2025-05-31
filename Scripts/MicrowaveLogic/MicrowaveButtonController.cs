using UnityEngine;

public class MicrowaveButtonController : MonoBehaviour, IInteractable
{
    [SerializeField] private MicrowaveController microwaveController;

    public bool CanInteract => true;

    public void Interact() => microwaveController.StartCooking();
}