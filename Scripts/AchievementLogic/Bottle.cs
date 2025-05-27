using UnityEngine;

public sealed class Bottle : MonoBehaviour
{
    [SerializeField] private AchievementData _linkedAchievement;
    [SerializeField] private AchievementManager _manager;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<Cup>() != null)
        {
            _manager.ReportHit(_linkedAchievement);
        }
    }
}