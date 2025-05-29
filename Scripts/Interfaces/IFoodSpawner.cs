using UnityEngine;

public interface IFoodSpawner
{
    GameObject SpawnItem(ItemType type, Vector3 position);
    void ReturnItem(GameObject item);
}
