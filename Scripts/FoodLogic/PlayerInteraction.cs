using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float interactionRange = 5f;
    [SerializeField] private LayerMask interactionMask = -1;

    private PlayerInventory inventory;

    private void Awake()
    {
        inventory = GetComponent<PlayerInventory>();
        if (!playerCamera) playerCamera = Camera.main;
    }

    private void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
            TryInteract();

        if (Input.GetMouseButtonDown(1) && inventory.HasItem)
            ThrowItem();
    }

    private void TryInteract()
    {
        if (!Physics.Raycast(playerCamera.ScreenPointToRay(Input.mousePosition),
            out RaycastHit hit, interactionRange, interactionMask)) return;

        var interactable = hit.collider.GetComponent<IItem>();
        interactable?.OnInteract();

        var station = hit.collider.GetComponent<IProcessingStation>();
        if (station != null)
            hit.collider.SendMessage("OnMouseDown", SendMessageOptions.DontRequireReceiver);

        var spawner = hit.collider.GetComponent<ItemSpawner>();
        if (spawner != null)
            hit.collider.SendMessage("OnMouseDown", SendMessageOptions.DontRequireReceiver);
    }

    private void ThrowItem()
    {
        var throwDirection = playerCamera.transform.forward;
        inventory.ThrowItem(throwDirection);
    }
}