//using UnityEngine;
//using Zenject;

//public sealed class PickupableItem : MonoBehaviour, IInteractable
//{
//    [Inject] private IItemManager _itemManager;

//    public void Interact()
//    {
//        if (_itemManager.HasItem) return;
//        _itemManager.TakeItem(gameObject);
//    }
//}
