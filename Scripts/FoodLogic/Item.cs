using System;
using System.Collections;
using UnityEngine;

public class Item : MonoBehaviour, IItem
{
    [SerializeField] private ItemType itemType;
    [SerializeField] private ItemState currentState = ItemState.Available;
    [SerializeField] private GameObject[] stateVisuals;

    public ItemType Type => itemType;
    public ItemState State => currentState;
    public GameObject GameObject => gameObject;

    private Action<ItemState> onStateChanged;

    public void ChangeState(ItemState newState)
    {
        if (currentState == newState) return;

        currentState = newState;
        UpdateVisuals();
        onStateChanged?.Invoke(newState);

        if (newState == ItemState.Thrown)
            StartCoroutine(AutoDestroy());
    }

    public bool CanInteract() =>
        currentState == ItemState.Available ||
        currentState == ItemState.Ready;

    public virtual void OnInteract()
    {
        if (!CanInteract()) return;

        var inventory = FindObjectOfType<PlayerInventory>();
        if (inventory.TryTakeItem(this))
            ChangeState(ItemState.InHand);
    }

    private void UpdateVisuals()
    {
        for (int i = 0; i < stateVisuals.Length; i++)
            if (stateVisuals[i]) stateVisuals[i].SetActive(i == (int)currentState);
    }

    private IEnumerator AutoDestroy()
    {
        yield return new WaitForSeconds(2f);
        Debug.Log($"{Type} item disappeared");
        Destroy(gameObject);
    }
}
