using UnityEngine;

public class Drink : Food
{
    public override ItemType ItemType => ItemType.Drink;

    protected override void OnConfigureInHand()
    {
        transform.localRotation = Quaternion.Euler(90f, 0, 0);
    }
}