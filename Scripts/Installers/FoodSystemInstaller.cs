using UnityEngine;
using Zenject;

public class FoodSystemInstaller : MonoInstaller
{
    [SerializeField] private FoodSpawner _foodSpawner;

    private void Awake()
    {
        if (_foodSpawner == null)
        {
            Debug.Log("FoodSpawner not assign", this);
            enabled = false;
            return;
        }
    }

    public override void InstallBindings()
    {
        Container.Bind<IFoodSpawner>().FromInstance(_foodSpawner).AsSingle();
    }
}