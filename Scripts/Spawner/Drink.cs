//public class Drink : Food
//{
//}


using UnityEngine;

public class Drink : Food
{
    protected override void OnConfigureInHand()
    {
        transform.localRotation = Quaternion.Euler(90f, 0, 0);
    }
}