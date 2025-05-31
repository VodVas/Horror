using UnityEngine;
using UnityEngine.Pool;
using Zenject;
using System.Collections.Generic;

public class FoodSpawner : MonoBehaviour, IFoodSpawner
{
    [SerializeField] private FoodPrefabMapping[] _prefabMappings;

    [Inject] private DiContainer _container;

    private readonly Dictionary<ItemType, Food> _prefabs = new();
    private readonly Dictionary<ItemType, ObjectPool<Food>> _pools = new();

    private void Awake()
    {
        InitializePrefabs();
        InitializePools();
    }

    private void InitializePrefabs()
    {
        foreach (var mapping in _prefabMappings)
        {
            _prefabs[mapping.type] = mapping.prefab;
        }
    }

    private void InitializePools()
    {
        foreach (var kvp in _prefabs)
        {
            var type = kvp.Key;
            _pools[type] = new ObjectPool<Food>(
                () => CreateFood(type),
                OnGetFromPool,
                OnReleaseToPool,
                OnDestroyFood
            );
        }
    }

    public GameObject SpawnItem(ItemType type, Vector3 position)
    {
        if (!_pools.TryGetValue(type, out var pool)) return null;

        var food = pool.Get();
        food.transform.position = position;
        food.ChangeState(ItemState.Available);
        return food.gameObject;
    }

    public void ReturnItem(GameObject item)
    {
        if (!item.TryGetComponent<Food>(out var food)) return;

        var type = GetItemType(food);
        if (_pools.TryGetValue(type, out var pool))
        {
            pool.Release(food);
        }
    }

    private Food CreateFood(ItemType type)
    {
        if (!_prefabs.TryGetValue(type, out var prefab)) return null;

        var instance = _container.InstantiatePrefabForComponent<Food>(prefab);
        instance.Terminated += HandleFoodTerminated;
        return instance;
    }

    private void OnGetFromPool(Food food)
    {
        food.gameObject.SetActive(true);
    }

    private void OnReleaseToPool(Food food)
    {
        food.ResetForPool();
    }

    private void OnDestroyFood(Food food)
    {
        if (food != null)
        {
            food.Terminated -= HandleFoodTerminated;
            Destroy(food.gameObject);
        }
    }

    private void HandleFoodTerminated(ITerminatable terminatable)
    {
        if (terminatable is Food food)
            ReturnItem(food.gameObject);
    }

    private ItemType GetItemType(Food food)
    {
        return food switch
        {
            Cup => ItemType.Cup,
            Drink => ItemType.Drink,
            Sandwich => ItemType.Sandwich,
            _ => ItemType.Cup
        };
    }
}