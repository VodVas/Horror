using System;
using UnityEngine;

public abstract class Food : MonoBehaviour, IPickupable, ITerminatable, IResettable, IInteractable
{
    public event Action<ITerminatable> Terminated;

    public void Interact()
    {

    }

    public virtual void ResetState()
    {
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        if (TryGetComponent<Rigidbody>(out var rb))
        {
            rb.isKinematic = false;
        }
    }

    public void ReturnToPool()
    {
        Terminated?.Invoke(this);
    }
}

//using System;
//using UnityEngine;

//public abstract class Food : MonoBehaviour, ITerminatable, IResettable
//{
//    public event Action<ITerminatable> Terminated;

//    public virtual void ResetState()
//    {
//        transform.localPosition = Vector3.zero;
//        transform.localRotation = Quaternion.identity;

//        if (TryGetComponent<Rigidbody>(out var rb))
//            rb.isKinematic = false;
//    }

//    public void ReturnToPool() => Terminated?.Invoke(this);
//}
