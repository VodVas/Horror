//public class Test
//{
//    //using UnityEngine;
//    //using VodVas.InterfaceSerializer;

//    //public class QueueFoodController : MonoBehaviour
//    //{
//    //    [Header("Cup Settings")]
//    //    [SerializeField] private Transform _targetCup;
//    //    [SerializeField] private GameObject _cupPrefab;
//    //    [SerializeField] private Transform _cupHoldPosition;

//    //    [Header("Throw Settings")]
//    //    [SerializeField] private float _throwForce = 10f;

//    //    [Header("Pickup Settings")]
//    //    [SerializeField] private float _pickupRadius = 1.5f;
//    //    [SerializeField] private float _pickupAngle = 45f;

//    //    [SerializeField, InterfaceConstraint(typeof(IInputProvider))]
//    //    private MonoBehaviour _inputProvider;

//    //    private GameObject _cupInHand;
//    //    private bool _hasCupInHand = false;
//    //    private IInputProvider _input => _inputProvider as IInputProvider;

//    //    private void Update()
//    //    {
//    //        if (_input.GetPushInput())
//    //        {
//    //            if (_hasCupInHand)
//    //            {
//    //                ThrowCup();
//    //            }
//    //            else
//    //            {
//    //                TryTakeCup();
//    //            }
//    //        }
//    //    }

//    //    private void TryTakeCup()
//    //    {
//    //        if (_hasCupInHand) return;

//    //        // Check table cup
//    //        if (_targetCup != null && _targetCup.gameObject.activeSelf)
//    //        {
//    //            Vector3 toTarget = _targetCup.position - transform.position;
//    //            if (toTarget.magnitude <= _pickupRadius)
//    //            {
//    //                float angle = Vector3.Angle(transform.forward, toTarget.normalized);
//    //                if (angle <= _pickupAngle)
//    //                {
//    //                    TakeNewCup();
//    //                    return;
//    //                }
//    //            }
//    //        }

//    //        Collider[] cups = Physics.OverlapSphere(transform.position, _pickupRadius, LayerMask.GetMask("Default"));
//    //        GameObject closestCup = null;
//    //        float minDist = Mathf.Infinity;

//    //        foreach (Collider col in cups)
//    //        {
//    //            if (!col.CompareTag("Cup")) continue;

//    //            Rigidbody rb = col.GetComponent<Rigidbody>();
//    //            if (rb == null || rb.isKinematic) continue;

//    //            Vector3 dir = col.transform.position - transform.position;
//    //            float angle = Vector3.Angle(transform.forward, dir.normalized);
//    //            if (angle > _pickupAngle) continue;

//    //            float dist = dir.sqrMagnitude;
//    //            if (dist < minDist)
//    //            {
//    //                minDist = dist;
//    //                closestCup = col.gameObject;
//    //            }
//    //        }

//    //        if (closestCup != null)
//    //        {
//    //            _cupInHand = closestCup;
//    //            PickUpThrownCup();
//    //        }
//    //    }

//    //    private void TakeNewCup()
//    //    {
//    //        if (_cupPrefab == null || _cupHoldPosition == null) return;

//    //        _cupInHand = Instantiate(_cupPrefab, _cupHoldPosition.position, _cupHoldPosition.rotation, _cupHoldPosition);
//    //        _cupInHand.SetActive(true);
//    //        ResetCupInHand();
//    //        _hasCupInHand = true;
//    //    }

//    //    private void PickUpThrownCup()
//    //    {
//    //        if (_cupInHand == null) return;

//    //        _cupInHand.SetActive(true);
//    //        ResetCupInHand();
//    //        _hasCupInHand = true;
//    //    }

//    //    private void ResetCupInHand()
//    //    {
//    //        _cupInHand.transform.SetParent(_cupHoldPosition);
//    //        _cupInHand.transform.localPosition = Vector3.zero;
//    //        _cupInHand.transform.localRotation = Quaternion.identity;

//    //        if (_cupInHand.TryGetComponent(out Rigidbody rb))
//    //        {
//    //            rb.isKinematic = true;
//    //            rb.velocity = Vector3.zero;
//    //            rb.angularVelocity = Vector3.zero;
//    //        }

//    //        _cupInHand.layer = LayerMask.NameToLayer("Weapon");
//    //    }

//    //    private void ThrowCup()
//    //    {
//    //        if (!_hasCupInHand || _cupInHand == null) return;

//    //        _cupInHand.layer = LayerMask.NameToLayer("Default");
//    //        if (_cupInHand.TryGetComponent(out Rigidbody rb))
//    //        {
//    //            rb.isKinematic = false;
//    //            rb.AddForce(transform.forward * _throwForce, ForceMode.Impulse);
//    //        }

//    //        _cupInHand.transform.SetParent(null);
//    //        _hasCupInHand = false;
//    //        _cupInHand = null;
//    //    }
//    //}

//    /*//using UnityEngine;
//    //using VodVas.InterfaceSerializer;

//    //public class QueueFoodController : MonoBehaviour
//    //{
//    //    [Header("Spawn Settings")]
//    //    [SerializeField] private ItemSpawnPoint[] _targetSpawnPoints;
//    //    [SerializeField] private ItemTypePrefab[] _itemPrefabs;
//    //    [SerializeField] private Transform _itemHoldPosition;

//    //    [Header("Throw Settings")]
//    //    [SerializeField] private float _throwForce = 10f;

//    //    [Header("Pickup Settings")]
//    //    [SerializeField] private float _pickupRadius = 2f;
//    //    [SerializeField] private float _pickupAngle = 10f;

//    //    [SerializeField, InterfaceConstraint(typeof(IInputProvider))]
//    //    private MonoBehaviour _inputProvider;

//    //    private GameObject _itemInHand;
//    //    private bool _hasItemInHand = false;
//    //    private IInputProvider _input => _inputProvider as IInputProvider;

//    //    private void Update()
//    //    {
//    //        if (_input.GetPushInput())
//    //        {
//    //            if (_hasItemInHand) ThrowItem();
//    //            else TryTakeItem();
//    //        }
//    //    }

//    //    private void TryTakeItem()
//    //    {
//    //        if (_hasItemInHand) return;

//    //        ItemSpawnPoint closestSpawn = GetClosestSpawnPoint();

//    //        if (closestSpawn != null)
//    //        {
//    //            GameObject prefab = GetPrefabByType(closestSpawn.itemType);
//    //            if (prefab != null) TakeNewItem(prefab);
//    //            return;
//    //        }

//    //        GameObject closestItem = FindNearestItemInRadius();
//    //        if (closestItem != null) PickUpThrownItem(closestItem);
//    //    }

//    //    private ItemSpawnPoint GetClosestSpawnPoint()
//    //    {
//    //        ItemSpawnPoint closest = null;
//    //        float minDistance = Mathf.Infinity;

//    //        foreach (var spawnPoint in _targetSpawnPoints)
//    //        {
//    //            if (spawnPoint == null || !spawnPoint.gameObject.activeSelf) continue;

//    //            Vector3 toPoint = spawnPoint.transform.position - transform.position;
//    //            float distance = toPoint.magnitude;
//    //            if (distance > _pickupRadius) continue;

//    //            float angle = Vector3.Angle(transform.forward, toPoint.normalized);
//    //            if (angle > _pickupAngle) continue;

//    //            if (distance < minDistance)
//    //            {
//    //                minDistance = distance;
//    //                closest = spawnPoint;
//    //            }
//    //        }
//    //        return closest;
//    //    }

//    //    private GameObject GetPrefabByType(ItemType type)
//    //    {
//    //        foreach (var pair in _itemPrefabs)
//    //        {
//    //            if (pair.type == type) return pair.prefab;
//    //        }
//    //        Debug.LogError($"No prefab for type {type}");
//    //        return null;
//    //    }

//    //    private GameObject FindNearestItemInRadius()
//    //    {
//    //        Collider[] colliders = Physics.OverlapSphere(transform.position, _pickupRadius);
//    //        GameObject closest = null;
//    //        float minDist = Mathf.Infinity;

//    //        foreach (Collider col in colliders)
//    //        {
//    //            if (!col.TryGetComponent<IPickupable>(out var item)) continue;

//    //            Vector3 dir = col.transform.position - transform.position;
//    //            float angle = Vector3.Angle(transform.forward, dir.normalized);
//    //            if (angle > _pickupAngle) continue;

//    //            float dist = dir.sqrMagnitude;
//    //            if (dist < minDist)
//    //            {
//    //                minDist = dist;
//    //                closest = col.gameObject;
//    //            }
//    //        }
//    //        return closest;
//    //    }

//    //    private void TakeNewItem(GameObject prefab)
//    //    {
//    //        _itemInHand = Instantiate(prefab, _itemHoldPosition.position, _itemHoldPosition.rotation, _itemHoldPosition);

//    //        _itemInHand.layer = LayerMask.NameToLayer("Weapon");
//    //        _itemInHand.SetActive(true);
//    //        ResetItemInHand();
//    //        _hasItemInHand = true;
//    //    }

//    //    private void PickUpThrownItem(GameObject item)
//    //    {
//    //        _itemInHand = item;
//    //        ResetItemInHand();
//    //        _hasItemInHand = true;
//    //    }

//    //    private void ResetItemInHand()
//    //    {
//    //        _itemInHand.transform.SetParent(_itemHoldPosition);
//    //        _itemInHand.transform.localPosition = Vector3.zero;
//    //        _itemInHand.transform.localRotation = Quaternion.identity;

//    //        if (_itemInHand.TryGetComponent(out Rigidbody rb))
//    //        {
//    //            rb.isKinematic = true;

//    //            if (_itemInHand.TryGetComponent(out Drink drink))
//    //            {
//    //                drink.transform.localRotation *= Quaternion.Euler(90f, 0, 0);
//    //            }

//    //        }
//    //        _itemInHand.layer = LayerMask.NameToLayer("Weapon");
//    //    }

//    //    private void ThrowItem()
//    //    {
//    //        if (!_hasItemInHand || _itemInHand == null) return;

//    //        _itemInHand.layer = LayerMask.NameToLayer("Default");
//    //        if (_itemInHand.TryGetComponent(out Rigidbody rb))
//    //        {
//    //            rb.isKinematic = false;
//    //            rb.AddForce(transform.forward * _throwForce, ForceMode.Impulse);
//    //        }

//    //        _itemInHand.transform.SetParent(null);
//    //        _hasItemInHand = false;
//    //        _itemInHand = null;
//    //    }

//    //    private void OnDrawGizmos()
//    //    {
//    //        Gizmos.color = new Color(0, 1, 0, 0.3f);
//    //        Gizmos.DrawSphere(transform.position, _pickupRadius);

//    //        Vector3 forward = transform.forward;
//    //        Quaternion leftRotation = Quaternion.Euler(0, -_pickupAngle / 2, 0);
//    //        Quaternion rightRotation = Quaternion.Euler(0, _pickupAngle / 2, 0);

//    //        Vector3 leftDir = leftRotation * forward;
//    //        Vector3 rightDir = rightRotation * forward;

//    //        Gizmos.color = Color.green;
//    //        Gizmos.DrawRay(transform.position, forward * _pickupRadius);
//    //        Gizmos.DrawRay(transform.position, leftDir * _pickupRadius);
//    //        Gizmos.DrawRay(transform.position, rightDir * _pickupRadius);

//    //        if (_itemInHand != null)
//    //        {
//    //            Gizmos.color = Color.blue;
//    //            Gizmos.DrawLine(transform.position, _itemInHand.transform.position);
//    //            Gizmos.DrawWireSphere(_itemInHand.transform.position, 0.2f);
//    //        }
//    //    }
//    //}

//    //public enum ItemType { Cup, Food, Drink }*/
//}/*










////using UnityEngine;

////public class QueueFoodController : MonoBehaviour
////{
////    private const string Weapon = "Weapon";
////    private const string Default = "Default";

////    [Header("Spawn Settings")]
////    [SerializeField] private ItemSpawnPoint[] _targetSpawnPoints;
////    [SerializeField] private ItemTypePrefab[] _itemPrefabs;
////    [SerializeField] private Transform _itemHoldPosition;

////    [Header("Throw Settings")]
////    [SerializeField] private float _throwForce = 10f;

////    [Header("Pickup Settings")]
////    [SerializeField] private float _pickupRadius = 2f;
////    [SerializeField] private float _pickupAngle = 10f;

////    [SerializeField] private NewInputProvider _input;
////    [SerializeField] private PhysicsItemScanner _itemScanner;

////    private GameObject _itemInHand;
////    private bool _hasItemInHand = false;

////    private void Awake()
////    {
////        if (_itemScanner == null)
////        {
////            Debug.Log("PhysicsItemScanner not assign", this);
////            enabled = false;
////            return;
////        }
////    }

////    private void Update()
////    {
////        if (_input.GetPushInput())
////        {
////            if (_hasItemInHand) ThrowItem();
////            else TryTakeItem();
////        }
////    }

////    private void TryTakeItem()
////    {
////        if (_hasItemInHand) return;

////        ItemSpawnPoint closestSpawn = GetClosestSpawnPoint();

////        if (closestSpawn != null)
////        {
////            GameObject prefab = GetPrefabByType(closestSpawn.itemType);
////            if (prefab != null) TakeNewItem(prefab);
////            return;
////        }

////        GameObject closestItem = _itemScanner.FindNearestPickupable(
////            transform.position,
////            transform.forward,
////            _pickupRadius,
////            _pickupAngle);

////        if (closestItem != null) PickUpThrownItem(closestItem);
////    }

////    private ItemSpawnPoint GetClosestSpawnPoint()
////    {
////        ItemSpawnPoint closest = null;
////        float minDistance = Mathf.Infinity;

////        foreach (var spawnPoint in _targetSpawnPoints)
////        {
////            if (spawnPoint == null || !spawnPoint.gameObject.activeSelf) continue;

////            Vector3 toPoint = spawnPoint.transform.position - transform.position;
////            float distance = toPoint.magnitude;
////            if (distance > _pickupRadius) continue;

////            float angle = Vector3.Angle(transform.forward, toPoint.normalized);
////            if (angle > _pickupAngle) continue;

////            if (distance < minDistance)
////            {
////                minDistance = distance;
////                closest = spawnPoint;
////            }
////        }
////        return closest;
////    }

////    private GameObject GetPrefabByType(ItemType type)
////    {
////        foreach (var pair in _itemPrefabs)
////        {
////            if (pair.type == type) return pair.prefab;
////        }
////        Debug.LogError($"No prefab for type {type}");
////        return null;
////    }

////    private void TakeNewItem(GameObject prefab)
////    {
////        _itemInHand = Instantiate(prefab, _itemHoldPosition.position, _itemHoldPosition.rotation, _itemHoldPosition);

////        _itemInHand.layer = LayerMask.NameToLayer(Weapon);
////        _itemInHand.SetActive(true);
////        ResetItemInHand();
////        _hasItemInHand = true;
////    }

////    private void PickUpThrownItem(GameObject item)
////    {
////        _itemInHand = item;
////        ResetItemInHand();
////        _hasItemInHand = true;
////    }

////    private void ResetItemInHand()
////    {
////        _itemInHand.transform.SetParent(_itemHoldPosition);
////        _itemInHand.transform.localPosition = Vector3.zero;
////        _itemInHand.transform.localRotation = Quaternion.identity;

////        if (_itemInHand.TryGetComponent(out Rigidbody rb))
////        {
////            rb.isKinematic = true;

////            if (_itemInHand.TryGetComponent(out Drink drink))
////            {
////                drink.transform.localRotation *= Quaternion.Euler(90f, 0, 0);
////            }

////        }
////        _itemInHand.layer = LayerMask.NameToLayer(Weapon);
////    }

////    private void ThrowItem()
////    {
////        if (!_hasItemInHand || _itemInHand == null) return;

////        _itemInHand.layer = LayerMask.NameToLayer(Default);

////        if (_itemInHand.TryGetComponent(out Rigidbody rb))
////        {
////            rb.isKinematic = false;
////            rb.AddForce(transform.forward * _throwForce, ForceMode.Impulse);
////        }

////        _itemInHand.transform.SetParent(null);
////        _hasItemInHand = false;
////        _itemInHand = null;
////    }
////}*//*//using System.Collections;
////using UnityEngine;
////using Zenject;

////[RequireComponent(typeof(PhysicsItemScanner))]
////public sealed class QueueFoodController : MonoBehaviour
////{
////    const string WeaponLayer = "Weapon";
////    const string DefaultLayer = "Default";

////    [Header("Spawn Settings")]
////    [SerializeField] private ItemSpawnPoint[] _targetSpawnPoints;
////    [SerializeField] private Transform _itemHoldPosition;

////    [Header("Throw Settings")]
////    [SerializeField] private float _throwForce = 10f;

////    [Header("Pickup Settings")]
////    [SerializeField] private float _pickupRadius = 2f;
////    [SerializeField] private float _pickupAngle = 10f;

////    [Header("Auto Return Settings")]
////    [SerializeField] private float _autoReturnDelay = 50f;

////    [SerializeField] private NewInputProvider _input;

////    [Inject] private IFoodSpawner _foodSpawner;

////    private PhysicsItemScanner _itemScanner;
////    private GameObject _itemInHand;
////    private bool _hasItemInHand;

////    private void Awake()
////    {
////        _itemScanner = GetComponent<PhysicsItemScanner>();
////    }

////    private void Update()
////    {
////        if (_input.GetPushInput())
////        {
////            if (_hasItemInHand) ThrowItem();
////            else TryTakeItem();
////        }
////    }

////    private void TryTakeItem()
////    {
////        if (_hasItemInHand) return;

////        var closestSpawn = GetClosestSpawnPoint();
////        if (closestSpawn != null)
////        {
////            TakeNewItemFromSpawn(closestSpawn.itemType);
////            return;
////        }

////        var closestItem = _itemScanner.FindNearestPickupable(
////            transform.position, transform.forward, _pickupRadius, _pickupAngle);

////        if (closestItem != null) PickUpThrownItem(closestItem);
////    }

////    private ItemSpawnPoint GetClosestSpawnPoint()
////    {
////        ItemSpawnPoint closest = null;
////        float minDistance = Mathf.Infinity;

////        foreach (var spawnPoint in _targetSpawnPoints)
////        {
////            if (!spawnPoint || !spawnPoint.gameObject.activeSelf) continue;

////            var toPoint = spawnPoint.transform.position - transform.position;
////            var distance = toPoint.magnitude;

////            if (distance > _pickupRadius) continue;

////            var angle = Vector3.Angle(transform.forward, toPoint.normalized);
////            if (angle > _pickupAngle || distance >= minDistance) continue;

////            minDistance = distance;
////            closest = spawnPoint;
////        }

////        return closest;
////    }

////    private void TakeNewItemFromSpawn(ItemType itemType)
////    {
////        _itemInHand = _foodSpawner.SpawnItem(itemType, _itemHoldPosition.position);
////        if (_itemInHand == null) return;

////        ConfigureItemInHand();
////        _hasItemInHand = true;
////    }

////    private void PickUpThrownItem(GameObject item)
////    {
////        _itemInHand = item;
////        ConfigureItemInHand();
////        _hasItemInHand = true;
////    }

////    private void ConfigureItemInHand()
////    {
////        _itemInHand.layer = LayerMask.NameToLayer(WeaponLayer);
////        _itemInHand.transform.SetParent(_itemHoldPosition);
////        _itemInHand.transform.localPosition = Vector3.zero;
////        _itemInHand.transform.localRotation = Quaternion.identity;

////        if (_itemInHand.TryGetComponent<Rigidbody>(out var rb))
////        {
////            rb.isKinematic = true;
////        }

////        if (_itemInHand.TryGetComponent<Drink>(out _))
////        {
////            _itemInHand.transform.localRotation *= Quaternion.Euler(90f, 0, 0);
////        }
////    }

////    private void ThrowItem()
////    {
////        if (!_hasItemInHand || !_itemInHand) return;

////        _itemInHand.layer = LayerMask.NameToLayer(DefaultLayer);
////        _itemInHand.transform.SetParent(null);

////        if (_itemInHand.TryGetComponent<Rigidbody>(out var rb))
////        {
////            rb.isKinematic = false;
////            rb.AddForce(transform.forward * _throwForce, ForceMode.Impulse);
////        }

////        //StartCoroutine(AutoReturnToPool(_itemInHand));

////        _hasItemInHand = false;
////        _itemInHand = null;
////    }

////    private IEnumerator AutoReturnToPool(GameObject item)
////    {
////        yield return new WaitForSeconds(_autoReturnDelay);

////        if (item && item.activeInHierarchy)
////        {
////            _foodSpawner.ReturnItem(item);
////        }
////    }
////}
//*/