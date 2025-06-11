using UnityEngine;

public class Sandwich : Food, IRewardableFood
{
    private bool _isHeated = false;

    public override ItemType ItemType => ItemType.Sandwich;

    public bool IsRewardable()
    {
        return _isHeated;
    }

    public void MarkAsHeated()
    {
        _isHeated = true;
        Debug.Log("MarkAsHeated()" + _isHeated);
    }

    protected override void ResetSpecificState()
    {
        _isHeated = false;
    }
}