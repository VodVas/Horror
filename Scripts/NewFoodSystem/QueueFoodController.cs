//////using UnityEngine;
//////using Zenject;

//////[RequireComponent(typeof(PhysicsItemScanner))]
//////public sealed class QueueFoodController : MonoBehaviour
//////{
//////    [SerializeField] private ItemSpawnPoint[] _targetSpawnPoints;
//////    [SerializeField] private Transform _itemHoldPosition;
//////    [SerializeField] private float _throwForce = 10f;
//////    [SerializeField] private float _pickupRadius = 2f;
//////    [SerializeField] private float _pickupAngle = 10f;

//////    [Inject] private NewInputProvider _input;
//////    [Inject] private IFoodSpawner _foodSpawner;

//////    private ItemScannerService _scannerService;
//////    private ItemSpawnService _spawnService;
//////    private PositionalItemGripper _heldItemManager;
//////    private ThrowItemService _throwItemService;

//////    private void Awake()
//////    {
//////        var physicsScanner = GetComponent<PhysicsItemScanner>();
//////        _scannerService = gameObject.AddComponent<ItemScannerService>();
//////        _scannerService.Initialize(physicsScanner);

//////        _spawnService = new ItemSpawnService(_foodSpawner);
//////        _heldItemManager = new PositionalItemGripper(_itemHoldPosition);
//////        _throwItemService = new ThrowItemService(_throwForce);
//////    }

//////    private void Update()
//////    {
//////        if (_input.GetPushInput())
//////        {
//////            if (_heldItemManager.HasItem)
//////                ThrowItem();
//////            else
//////                TryTakeItem();
//////        }
//////    }

//////    private void TryTakeItem()
//////    {
//////        var closestSpawn = GetClosestSpawnPoint();

//////        if (closestSpawn != null)
//////        {
//////            var newItem = _spawnService.SpawnItem(closestSpawn.itemType, _itemHoldPosition.position);

//////            if (newItem != null)
//////            {
//////                _heldItemManager.TakeItem(newItem);
//////                return;
//////            }
//////        }

//////        var closestItem = _scannerService.FindNearestPickupable(transform.position, transform.forward, _pickupRadius, _pickupAngle);

//////        if (closestItem != null)
//////        {
//////            _heldItemManager.TakeItem(closestItem);
//////        }
//////    }

//////    private ItemSpawnPoint GetClosestSpawnPoint()
//////    {
//////        ItemSpawnPoint closest = null;
//////        float minDistance = _pickupRadius;

//////        for (int i = 0; i < _targetSpawnPoints.Length; i++)
//////        {
//////            var spawnPoint = _targetSpawnPoints[i];

//////            if (spawnPoint == null || !spawnPoint.gameObject.activeSelf) continue;

//////            Vector3 toPoint = spawnPoint.transform.position - transform.position;
//////            float distance = toPoint.sqrMagnitude;

//////            if (distance > minDistance * minDistance) continue;

//////            float angle = Vector3.Angle(transform.forward, toPoint.normalized);

//////            if (angle > _pickupAngle) continue;

//////            if (closest == null || distance < minDistance * minDistance)
//////            {
//////                minDistance = Mathf.Sqrt(distance);
//////                closest = spawnPoint;
//////            }
//////        }

//////        return closest;
//////    }

//////    private void ThrowItem()
//////    {
//////        var item = _heldItemManager.ReleaseItem();

//////        if (item == null) return;

//////        _throwItemService.Throw(item, transform.forward);
//////    }
//////}

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
