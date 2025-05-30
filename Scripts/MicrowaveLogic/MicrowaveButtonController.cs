using UnityEngine;

public class MicrowaveButtonController : MonoBehaviour, IInteractable
{
    [SerializeField] private MicrowaveController microwaveController;
    public void Interact() => microwaveController.StartCooking();
}