using UnityEngine;

public class CoffeeItem : Item
{
    [SerializeField] private GameObject cupInHand;
    [SerializeField] private GameObject cupWithLid;
    [SerializeField] private GameObject lid;

    private bool hasLid = false;

    public override void OnInteract()
    {
        var inventory = FindObjectOfType<PlayerInventory>();

        if (State == ItemState.Ready && !hasLid)
        {
            if (inventory.CurrentItem?.Type == ItemType.Coffee && (object)inventory.CurrentItem != this)
            {
                AttachLid();
                return;
            }
        }

        base.OnInteract();
    }

    private void AttachLid()
    {
        hasLid = true;
        lid.SetActive(false);
        cupWithLid.SetActive(true);

        var inventory = FindObjectOfType<PlayerInventory>();
        inventory.ClearItem();
        inventory.TryTakeItem(this);
    }
}
