//using UnityEngine;
//using Zenject;

//public sealed class PlayerItemHandler : MonoBehaviour, IInteractable
//{
//    [SerializeField] private float _interactionRadius = 2f;
//    [SerializeField] private float _interactionAngle = 10f;
//    [SerializeField] private LayerMask _spawnPointLayer;
//    [SerializeField] private LayerMask _pickupableLayer;

//    [Inject] private IItemManager _itemManager;
//    [Inject] private IItemSpawner _itemSpawner;

//    private readonly Collider[] _colliderBuffer = new Collider[32];

//    public void Interact()
//    {
//        if (_itemManager.HasItem)
//        {
//            _itemManager.ThrowItem();
//        }
//        else
//        {
//            TryTakeItem();
//        }
//    }

//    private void TryTakeItem()
//    {
//        var closestSpawnPoint = FindNearestSpawnPoint();

//        if (closestSpawnPoint != null)
//        {
//            var spawnedItem = _itemSpawner.SpawnItem(closestSpawnPoint.itemType, transform.position); // Spawn at player position for now, will adjust later if needed

//            if (spawnedItem != null)
//            {
//                _itemManager.TakeItem(spawnedItem);
//            }
//            return;
//        }

//        var closestPickupable = FindNearestPickupable();

//        if (closestPickupable != null)
//        {
//            _itemManager.TakeItem(closestPickupable);
//        }
//    }

//    private ItemSpawnPoint FindNearestSpawnPoint()
//    {
//        ItemSpawnPoint closest = null;
//        float minSqrDistance = _interactionRadius * _interactionRadius;

//        int count = Physics.OverlapSphereNonAlloc(transform.position, _interactionRadius, _colliderBuffer, _spawnPointLayer);

//        for (int i = 0; i < count; i++)
//        {
//            var col = _colliderBuffer[i];
//            if (col == null) continue;

//            if (col.TryGetComponent<ItemSpawnPoint>(out var spawnPoint))
//            {
//                Vector3 toPoint = col.transform.position - transform.position;
//                float sqrDistance = toPoint.sqrMagnitude;

//                if (sqrDistance > minSqrDistance) continue;

//                float angle = Vector3.Angle(transform.forward, toPoint.normalized);
//                if (angle > _interactionAngle) continue;

//                if (closest == null || sqrDistance < minSqrDistance)
//                {
//                    minSqrDistance = sqrDistance;
//                    closest = spawnPoint;
//                }
//            }
//        }
//        return closest;
//    }

//    private GameObject FindNearestPickupable()
//    {
//        GameObject closest = null;
//        float minSqrDistance = _interactionRadius * _interactionRadius;

//        int count = Physics.OverlapSphereNonAlloc(transform.position, _interactionRadius, _colliderBuffer, _pickupableLayer);

//        for (int i = 0; i < count; i++)
//        {
//            var col = _colliderBuffer[i];
//            if (col == null) continue;

//            if (col.TryGetComponent<IPickupable>(out _))
//            {
//                Vector3 toItem = col.transform.position - transform.position;
//                float sqrDistance = toItem.sqrMagnitude;

//                if (sqrDistance > minSqrDistance) continue;

//                float angle = Vector3.Angle(transform.forward, toItem.normalized);
//                if (angle > _interactionAngle) continue;

//                if (closest == null || sqrDistance < minSqrDistance)
//                {
//                    minSqrDistance = sqrDistance;
//                    closest = col.gameObject;
//                }
//            }
//        }
//        return closest;
//    }
//} 