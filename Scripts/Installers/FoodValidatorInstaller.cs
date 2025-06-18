//using UnityEngine;
//using Zenject;

//public class FoodValidatorInstaller : MonoInstaller
//{
//    [SerializeField] private FoodValidator _foodValidatorPrefab;

//    public override void InstallBindings()
//    {
//        Container.Bind<FoodValidator>().FromInstance(_foodValidatorPrefab).AsTransient().NonLazy();
//    }
//}