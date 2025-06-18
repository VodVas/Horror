using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public sealed class FoodValidator : MonoBehaviour
{
    public event Action<GameObject, bool> OnFoodValidated;
    public event Action<GameObject> OnValidFoodDetected;
    public event Action<GameObject> OnInvalidFoodDetected;

    private void OnTriggerEnter(Collider other)
    {
        if (other == null) return;

        var otherGameObject = other.gameObject;

        if (!otherGameObject.TryGetComponent(out IInteractable _)) return;

        bool isValid = CheckIfRewardable(other);

        OnFoodValidated?.Invoke(otherGameObject, isValid);

        if (isValid)
        {
            OnValidFoodDetected?.Invoke(otherGameObject);
        }
        else
        {
            OnInvalidFoodDetected?.Invoke(otherGameObject);
        }
    }

    private bool CheckIfRewardable(Collider other)
    {
        if (other.TryGetComponent(out IRewardableFood rewardableFood))
        {
            return rewardableFood.IsRewardable();
        }

        return false;
    }

    private void OnDestroy()
    {
        OnFoodValidated = null;
        OnValidFoodDetected = null;
        OnInvalidFoodDetected = null;
    }
}