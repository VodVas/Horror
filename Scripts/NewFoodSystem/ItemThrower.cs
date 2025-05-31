using UnityEngine;

public sealed class ItemThrower : MonoBehaviour, IInteractable
{
    [SerializeField] private HeldItemManager _heldItemManager;
    [SerializeField] private ThrowItemService _throwItemService;
    [SerializeField] private Transform _throwDirection;

    public bool CanInteract => _heldItemManager.HasItem;

    private void Awake()
    {
        if (_throwDirection == null) _throwDirection = transform;
    }

    public void Interact()
    {
        if (!CanInteract) return;

        var item = _heldItemManager.ReleaseItem();
        if (item != null)
        {
            item.ChangeState(ItemState.Thrown);
            _throwItemService.Throw(item.gameObject, _throwDirection.forward);
        }
    }
}