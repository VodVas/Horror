using UnityEngine;

public sealed class Bottle : MonoBehaviour
{
    [SerializeField] private AchievementData _linkedAchievement;
    [SerializeField] private AchievementManager _manager;
    [SerializeField] private float _hitCooldown = 0.5f;

    private float _lastHitTime;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<Food>() != null && Time.time > _lastHitTime + _hitCooldown)
        {
            _lastHitTime = Time.time;

            _manager.ReportHit(_linkedAchievement);
        }
    }
}