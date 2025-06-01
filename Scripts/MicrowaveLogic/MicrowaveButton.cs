using UnityEngine;

public sealed class MicrowaveButton : MonoBehaviour, IInteractable
{
    private MicrowaveController _controller;
    private bool _isInitialized;

    public bool CanInteract => _isInitialized &&
                              _controller.CurrentState == MicrowaveState.DoorClosed;

    public Transform Transform => GetComponent<Transform>();

    private void OnDestroy()
    {
        if (_controller) _controller.OnStateChanged -= HandleStateChanged;
    }

    public void Initialize(MicrowaveController controller)
    {
        _controller = controller;
        _controller.OnStateChanged += HandleStateChanged;
        _isInitialized = true;
    }

    public void Interact()
    {
        Debug.Log("Interact()");
        if (!CanInteract) return;
        _controller.RequestStartCooking();
    }

    private void HandleStateChanged(MicrowaveState newState)
    {
    }
}