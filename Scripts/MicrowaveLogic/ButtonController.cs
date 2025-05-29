using UnityEngine;

public class ButtonController : MonoBehaviour, IInteractable
{
    [SerializeField] private MicrowaveController microwaveController;

    [ContextMenu("Interact")]
    public void Interact() => microwaveController.StartCooking();
}
