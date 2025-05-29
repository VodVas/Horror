using UnityEngine;
using Zenject;

public class InputProviderInstaller : MonoInstaller
{
    [SerializeField] private NewInputProvider _input;

    private void Awake()
    {
        if (_input == null)
        {
            Debug.Log("NewInputProvider not assign", this);
            enabled = false;
            return;
        }
    }

    public override void InstallBindings()
    {
        Container.Bind<NewInputProvider>().FromInstance(_input).AsSingle().NonLazy();
    }
}