using UnityEngine;

public class Drink : Food, IRewardableFood
{
    public override ItemType ItemType => ItemType.Drink;

    public bool IsRewardable()
    {
        return true;
    }

    protected override void OnConfigureInHand()
    {
        transform.localRotation = Quaternion.Euler(90f, 0, 0);
    }
}