using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private int maxItems = 10;

    private int currentCount = 0;

    private void Start() => SpawnItem();

    private void OnMouseDown()
    {
        var inventory = FindObjectOfType<PlayerInventory>();
        if (inventory.HasItem) return;

        SpawnItem();
    }

    private void SpawnItem()
    {
        if (currentCount >= maxItems) return;

        var item = Instantiate(itemPrefab, spawnPoint.position, spawnPoint.rotation);
        currentCount++;

        var itemComponent = item.GetComponent<Item>();
        if (itemComponent)
            itemComponent.ChangeState(ItemState.Available);
    }
}
