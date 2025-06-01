using UnityEngine;

public interface IInteractionProjectile
{
    void Launch(Vector3 origin, Vector3 direction, float speed, System.Action<IInteractable> onHit);
}
