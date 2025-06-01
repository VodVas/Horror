using UnityEngine;
using Zenject;

public sealed class QueueFoodController : MonoBehaviour
{
    [SerializeField] private ItemSpawnPoint[] _targetSpawnPoints;
    [SerializeField] private HeldItemManager _heldItemManager;

    [Inject] private IFoodSpawner _foodSpawner;

    private void OnEnable()
    {
        foreach (var spawnPoint in _targetSpawnPoints)
        {
            if (spawnPoint != null)
                spawnPoint.OnSpawnRequested += HandleSpawnRequest;
        }
    }

    private void OnDisable()
    {
        foreach (var spawnPoint in _targetSpawnPoints)
        {
            if (spawnPoint != null)
                spawnPoint.OnSpawnRequested -= HandleSpawnRequest;
        }
    }

    private void HandleSpawnRequest(ItemType itemType, Vector3 spawnPosition)
    {
        if (_heldItemManager.HasItem) return;

        var itemObject = _foodSpawner.SpawnItem(itemType, spawnPosition);

        if (itemObject?.TryGetComponent<Food>(out var food) == true)
        {
            food.OnInteractionRequested += HandleFoodInteraction;
            _heldItemManager.TryTakeItem(food);
        }
    }

    private void HandleFoodInteraction(Food food)
    {
        if (!_heldItemManager.HasItem)
        {
            _heldItemManager.TryTakeItem(food);
        }
    }
}
