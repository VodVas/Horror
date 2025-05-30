//using UnityEngine;
//using Zenject;

//[RequireComponent(typeof(PhysicsItemScanner))]
//public sealed class QueueFoodController : MonoBehaviour
//{
//    [SerializeField] private ItemSpawnPoint[] _targetSpawnPoints;
//    [SerializeField] private Transform _itemHoldPosition;
//    [SerializeField] private float _throwForce = 10f;
//    [SerializeField] private float _pickupRadius = 2f;
//    [SerializeField] private float _pickupAngle = 10f;

//    [Inject] private NewInputProvider _input;
//    [Inject] private IFoodSpawner _foodSpawner;

//    private ItemScannerService _scannerService;
//    private ItemSpawnService _spawnService;
//    private PositionalItemGripper _heldItemManager;
//    private ThrowItemService _throwItemService;

//    private void Awake()
//    {
//        var physicsScanner = GetComponent<PhysicsItemScanner>();
//        _scannerService = gameObject.AddComponent<ItemScannerService>();
//        _scannerService.Initialize(physicsScanner);

//        _spawnService = new ItemSpawnService(_foodSpawner);
//        _heldItemManager = new PositionalItemGripper(_itemHoldPosition);
//        _throwItemService = new ThrowItemService(_throwForce);
//    }

//    private void Update()
//    {
//        if (_input.GetPushInput())
//        {
//            if (_heldItemManager.HasItem)
//                ThrowItem();
//            else
//                TryTakeItem();
//        }
//    }

//    private void TryTakeItem()
//    {
//        var closestSpawn = GetClosestSpawnPoint();

//        if (closestSpawn != null)
//        {
//            var newItem = _spawnService.SpawnItem(closestSpawn.itemType, _itemHoldPosition.position);

//            if (newItem != null)
//            {
//                _heldItemManager.TakeItem(newItem);
//                return;
//            }
//        }

//        var closestItem = _scannerService.FindNearestPickupable(transform.position, transform.forward, _pickupRadius, _pickupAngle);

//        if (closestItem != null)
//        {
//            _heldItemManager.TakeItem(closestItem);
//        }
//    }

//    private ItemSpawnPoint GetClosestSpawnPoint()
//    {
//        ItemSpawnPoint closest = null;
//        float minDistance = _pickupRadius;

//        for (int i = 0; i < _targetSpawnPoints.Length; i++)
//        {
//            var spawnPoint = _targetSpawnPoints[i];

//            if (spawnPoint == null || !spawnPoint.gameObject.activeSelf) continue;

//            Vector3 toPoint = spawnPoint.transform.position - transform.position;
//            float distance = toPoint.sqrMagnitude;

//            if (distance > minDistance * minDistance) continue;

//            float angle = Vector3.Angle(transform.forward, toPoint.normalized);

//            if (angle > _pickupAngle) continue;

//            if (closest == null || distance < minDistance * minDistance)
//            {
//                minDistance = Mathf.Sqrt(distance);
//                closest = spawnPoint;
//            }
//        }

//        return closest;
//    }

//    private void ThrowItem()
//    {
//        var item = _heldItemManager.ReleaseItem();

//        if (item == null) return;

//        _throwItemService.Throw(item, transform.forward);
//    }
//}










/*
using UnityEngine;
using Zenject;

public sealed class QueueFoodController : MonoBehaviour
{
    const string WeaponLayer = "Weapon";
    const string DefaultLayer = "Default";

    [Header("Spawn Settings")]
    [SerializeField] private ItemSpawnPoint[] _targetSpawnPoints;
    [SerializeField] private Transform _itemHoldPosition;

    [Header("Throw Settings")]
    [SerializeField] private float _throwForce = 10f;

    [Header("Pickup Settings")]
    [SerializeField] private float _pickupRadius = 2f;
    [SerializeField] private float _pickupAngle = 10f;

    [Inject] private NewInputProvider _input;
    [Inject] private IFoodSpawner _foodSpawner;

    private PhysicsItemScanner _itemScanner;
    private GameObject _itemInHand;
    private bool _hasItemInHand;

    private void Awake()
    {
        _itemScanner = GetComponent<PhysicsItemScanner>();
    }

    private void Update()
    {
        if (_input.GetPushInput())
        {
            if (_hasItemInHand) ThrowItem();
            else TryTakeItem();
        }
    }

    private void TryTakeItem()
    {
        if (_hasItemInHand) return;

        var closestSpawn = GetClosestSpawnPoint();

        if (closestSpawn != null)
        {
            TakeNewItemFromSpawn(closestSpawn.itemType);
            return;
        }

        var closestItem = _itemScanner.FindNearestPickupable(
            transform.position, transform.forward, _pickupRadius, _pickupAngle);

        if (closestItem != null) PickUpThrownItem(closestItem);
    }

    private ItemSpawnPoint GetClosestSpawnPoint()
    {
        ItemSpawnPoint closest = null;
        float minDistance = Mathf.Infinity;

        foreach (var spawnPoint in _targetSpawnPoints)
        {
            if (!spawnPoint || !spawnPoint.gameObject.activeSelf) continue;
            var toPoint = spawnPoint.transform.position - transform.position;
            var distance = toPoint.magnitude;

            if (distance > _pickupRadius) continue;

            var angle = Vector3.Angle(transform.forward, toPoint.normalized);
            if (angle > _pickupAngle || distance >= minDistance) continue;

            minDistance = distance;
            closest = spawnPoint;
        }

        return closest;
    }

    private void TakeNewItemFromSpawn(ItemType itemType)
    {
        _itemInHand = _foodSpawner.SpawnItem(itemType, _itemHoldPosition.position);

        if (_itemInHand == null) return;

        ConfigureItemInHand();
        _hasItemInHand = true;
    }

    private void PickUpThrownItem(GameObject item)
    {
        _itemInHand = item;
        ConfigureItemInHand();
        _hasItemInHand = true;
    }

    private void ConfigureItemInHand()
    {
        _itemInHand.layer = LayerMask.NameToLayer(WeaponLayer);
        _itemInHand.transform.SetParent(_itemHoldPosition);
        _itemInHand.transform.localPosition = Vector3.zero;
        _itemInHand.transform.localRotation = Quaternion.identity;

        if (_itemInHand.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.isKinematic = true;
        }

        if (_itemInHand.TryGetComponent<Drink>(out _))
        {
            _itemInHand.transform.localRotation *= Quaternion.Euler(90f, 0, 0);
        }
    }

    private void ThrowItem()
    {
        if (!_hasItemInHand || !_itemInHand) return;

        _itemInHand.layer = LayerMask.NameToLayer(DefaultLayer);
        _itemInHand.transform.SetParent(null);

        if (_itemInHand.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.isKinematic = false;
            rb.AddForce(transform.forward * _throwForce, ForceMode.Impulse);
        }

        _hasItemInHand = false;
        _itemInHand = null;
    }
}*/







//using UnityEngine;
//using Zenject;

////[RequireComponent(typeof(PhysicsItemScanner))]
//public sealed class QueueFoodController : MonoBehaviour
//{
//    [SerializeField] private ItemSpawnPoint[] _targetSpawnPoints;
//    [SerializeField] private Transform _itemHoldPosition;

//    [SerializeField] private float _throwForce = 10f;
//    [SerializeField] private float _pickupRadius = 2f;
//    [SerializeField] private float _pickupAngle = 10f;

//    [Inject] private NewInputProvider _input;
//    [Inject] private IFoodSpawner _foodSpawner;

//    //private PhysicsItemScanner _scannerService;
//    private ItemSpawnService _spawnService;
//    private HeldItemManager _heldItemManager;
//    private ThrowItemService _throwItemService;
//    private IInteractable _currentInteractable;

//    private void Awake()
//    {
//        //_scannerService = GetComponent<PhysicsItemScanner>();

//        _spawnService = new ItemSpawnService(_foodSpawner);
//        _heldItemManager = new HeldItemManager(_itemHoldPosition);
//        _throwItemService = new ThrowItemService(_throwForce);
//    }

//    //private void Update()
//    //{
//    //    if (_input.GetPushInput())
//    //    {
//    //        if (_heldItemManager.HasItem)
//    //            ThrowItem();
//    //        else
//    //            TryTakeItem();
//    //    }
//    //}

//    private void Update()
//    {
//        if (_heldItemManager.HasItem)
//            ThrowItem();
//        else
//            TryTakeItem();
//    }

//    private void OnEnable()
//    {
//        foreach (var spawnPoint in _targetSpawnPoints)
//        {
//            if (spawnPoint != null)
//                spawnPoint.OnSpawnRequested += HandleSpawnRequest;
//        }
//    }

//    private void OnDisable()
//    {
//        foreach (var spawnPoint in _targetSpawnPoints)
//        {
//            if (spawnPoint != null)
//                spawnPoint.OnSpawnRequested -= HandleSpawnRequest;
//        }
//    }

//    private void HandleSpawnRequest(ItemType itemType)
//    {
//        var newItem = _spawnService.SpawnItem(itemType, _itemHoldPosition.position);

//        if (newItem != null)
//            _heldItemManager.TakeItem(newItem);
//    }

//    private void TryTakeItem()
//    {
//        _currentInteractable.Interact();
//    }

//    //private void TryTakeItem()
//    //{
//    //    var closestSpawn = GetClosestSpawnPoint();
//    //    if (closestSpawn != null)
//    //    {
//    //        var newItem = _spawnService.SpawnItem(closestSpawn.itemType, _itemHoldPosition.position);
//    //        if (newItem != null)
//    //        {
//    //            _heldItemManager.TakeItem(newItem);
//    //            return;
//    //        }
//    //    }

//    //    var closestItem = _scannerService.FindNearestPickupable(transform.position, transform.forward, _pickupRadius, _pickupAngle);
//    //    if (closestItem != null)
//    //    {
//    //        _heldItemManager.TakeItem(closestItem);
//    //    }
//    //}

//    //private ItemSpawnPoint GetClosestSpawnPoint()
//    //{
//    //    ItemSpawnPoint closest = null;
//    //    float minDistance = _pickupRadius; // сразу ограничиваем радиус

//    //    for (int i = 0; i < _targetSpawnPoints.Length; i++)
//    //    {
//    //        var spawnPoint = _targetSpawnPoints[i];
//    //        if (spawnPoint == null || !spawnPoint.gameObject.activeSelf) continue;

//    //        Vector3 toPoint = spawnPoint.transform.position - transform.position;
//    //        float distance = toPoint.sqrMagnitude; // используем sqrMagnitude для оптимизации

//    //        if (distance > minDistance * minDistance) continue;

//    //        float angle = Vector3.Angle(transform.forward, toPoint.normalized);
//    //        if (angle > _pickupAngle) continue;

//    //        if (closest == null || distance < minDistance * minDistance)
//    //        {
//    //            minDistance = Mathf.Sqrt(distance);
//    //            closest = spawnPoint;
//    //        }
//    //    }

//    //    return closest;
//    //}

//    private void ThrowItem()
//    {
//        var item = _heldItemManager.ReleaseItem();
//        if (item == null) return;

//        _throwItemService.Throw(item, transform.forward);
//    }
//}



using UnityEngine;
using Zenject;

public sealed class QueueFoodController : MonoBehaviour
{
    [SerializeField] private ItemSpawnPoint[] _targetSpawnPoints;
    [SerializeField] private Transform _itemHoldPosition;
    [SerializeField] private float _throwForce = 10f;

    [Inject] private IFoodSpawner _foodSpawner;

    private ItemSpawnService _spawnService;
    private HeldItemManager _heldItemManager;
    private ThrowItemService _throwItemService;
    private IInteractable _currentInteractable;

    private void Awake()
    {
        _spawnService = new ItemSpawnService(_foodSpawner);
        _heldItemManager = new HeldItemManager(_itemHoldPosition);
        _throwItemService = new ThrowItemService(_throwForce);
    }

    private void Update()
    {
        if (_heldItemManager.HasItem)
            ThrowItem();
        else
            TryTakeItem();
    }

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

    private void HandleSpawnRequest(ItemType itemType)
    {
        Debug.Log("HandleSpawnRequest");
        var newItem = _spawnService.SpawnItem(itemType, _itemHoldPosition.position);

        if (newItem != null)
            _heldItemManager.TakeItem(newItem);
    }

    private void TryTakeItem()
    {
        _currentInteractable?.Interact();
    }

    private void ThrowItem()
    {
        var item = _heldItemManager.ReleaseItem();
        if (item == null) return;

        _throwItemService.Throw(item, transform.forward);
        _currentInteractable = null;
    }
}