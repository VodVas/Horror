//using UnityEngine;
//using Zenject;

//public sealed class SpawnPointInteractable : MonoBehaviour, IInteractable
//{
//    [SerializeField] private ItemType _itemType;
//    [SerializeField] private Transform _spawnPosition;

//    [Inject] private IItemManager _itemManager;
//    [Inject] private IItemSpawner _itemSpawner;

//    public void Interact()
//    {
//        if (_itemManager.HasItem) return;

//        var spawnedItem = _itemSpawner.SpawnItem(_itemType, _spawnPosition.position);

//        if (spawnedItem != null)
//            _itemManager.TakeItem(spawnedItem);
//    }
//}
