using System.Collections;
using UnityEngine;

public class CoffeeMachine : ProcessingStation
{
    [SerializeField] private GameObject brewingEffect;

    protected override void OnProcessingComplete()
    {
        base.OnProcessingComplete();
        if (brewingEffect) brewingEffect.SetActive(false);
    }

    protected override IEnumerator ProcessingCoroutine()
    {
        if (brewingEffect) brewingEffect.SetActive(true);
        yield return base.ProcessingCoroutine();
    }
}