using System;
using System.Collections;
using UnityEngine;

public abstract class ProcessingStation : MonoBehaviour, IProcessingStation
{
    [SerializeField] protected float processingTime = 3f;
    [SerializeField] protected ItemType[] supportedItems;
    [SerializeField] protected Transform itemSlot;

    protected IItem currentItem;
    protected bool isProcessing;
    protected float processingStartTime;

    public virtual bool CanProcess(IItem item) =>
        currentItem == null &&
        Array.Exists(supportedItems, x => x == item.Type);

    public virtual void StartProcessing(IItem item)
    {
        if (!CanProcess(item)) return;

        currentItem = item;
        isProcessing = true;
        processingStartTime = Time.time;

        item.GameObject.transform.SetParent(itemSlot);
        item.GameObject.transform.localPosition = Vector3.zero;
        item.ChangeState(ItemState.Processing);

        StartCoroutine(ProcessingCoroutine());
    }

    public bool IsProcessingComplete() =>
        currentItem != null && !isProcessing;

    public virtual IItem GetProcessedItem()
    {
        if (!IsProcessingComplete()) return null;

        var item = currentItem;
        currentItem = null;
        item.ChangeState(ItemState.Ready);
        return item;
    }

    protected virtual IEnumerator ProcessingCoroutine()
    {
        yield return new WaitForSeconds(processingTime);
        isProcessing = false;
        OnProcessingComplete();
    }

    protected virtual void OnProcessingComplete()
    {
        Debug.Log($"{currentItem.Type} processing complete!");
    }

    protected virtual void OnMouseDown()
    {
        var inventory = FindObjectOfType<PlayerInventory>();

        if (inventory.HasItem && CanProcess(inventory.CurrentItem))
        {
            StartProcessing(inventory.CurrentItem);
            inventory.ClearItem();
        }
        else if (IsProcessingComplete())
        {
            if (inventory.TryTakeItem(GetProcessedItem()))
                GetProcessedItem()?.ChangeState(ItemState.InHand);
        }
    }
}
