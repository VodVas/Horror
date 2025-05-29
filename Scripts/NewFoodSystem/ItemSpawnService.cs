using UnityEngine;

public sealed class ItemSpawnService
{
    private readonly IFoodSpawner _foodSpawner;

    public ItemSpawnService(IFoodSpawner foodSpawner)
    {
        _foodSpawner = foodSpawner;
    }

    public GameObject SpawnItem(ItemType itemType, Vector3 position)
    {
        return _foodSpawner.SpawnItem(itemType, position);
    }

    public void ReturnItem(GameObject item)
    {
        _foodSpawner.ReturnItem(item);
    }
}
