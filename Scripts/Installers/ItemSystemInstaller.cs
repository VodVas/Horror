//using UnityEngine;
//using Zenject;

//public class ItemSystemInstaller : MonoInstaller
//{
//    public override void InstallBindings()
//    {
//        Container.Bind<IItemSpawner>()
//            .FromComponentsInHierarchy()
//            .AsTransient();

//        Container.Bind<IItemManager>()
//            .FromComponentInHierarchy()
//            .AsSingle()
//            .NonLazy();
//    }
//}