using System.Collections.Generic;
using UnityEngine;

public sealed class InteractionProjectilePool : MonoBehaviour
{
    [SerializeField] private InteractionProjectile _projectilePrefab;
    [SerializeField] private int _poolSize = 5;

    private Queue<InteractionProjectile> _pool = new Queue<InteractionProjectile>();

    private void Awake()
    {
        for (int i = 0; i < _poolSize; i++)
        {
            var projectile = Instantiate(_projectilePrefab, transform);
            projectile.Initialize(ReturnProjectile);
            projectile.gameObject.SetActive(false);
            _pool.Enqueue(projectile);
        }
    }

    public bool TryGetProjectile(out InteractionProjectile projectile)
    {
        projectile = _pool.Count > 0 ? _pool.Dequeue() : null;
        return projectile != null;
    }

    private void ReturnProjectile(InteractionProjectile projectile)
    {
        _pool.Enqueue(projectile);
    }
}
