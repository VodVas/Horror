using UnityEngine;
using Zenject;

public sealed class PlayerInteractionController : MonoBehaviour
{
    [SerializeField] private float _projectileSpeed = 20f;
    [SerializeField] private float _interactionAngle = 30f;
    [SerializeField] private LayerMask _interactionLayer;
    [SerializeField] private LayerMask _obstacleLayer;
    [SerializeField] private Camera _camera;
    [SerializeField] private HeldItemManager _heldItemManager;
    [SerializeField] private InteractionProjectilePool _projectilePool;

    [Inject] private NewInputProvider _inputProvider;

    private IInteractable[] _playerInteractables;
    private InteractionTargetDetector _targetDetector;
    private bool _isProjectileActive;

    private void Awake()
    {
        _playerInteractables = GetComponentsInChildren<IInteractable>();
        _targetDetector = new InteractionTargetDetector(_obstacleLayer, _interactionAngle);
    }

    private void Update()
    {
        if (!_inputProvider.GetPushInput() || _isProjectileActive)
            return;

        if (_heldItemManager.HasItem && TryFindPlayerInteractable<ItemThrower>(out var thrower))
        {
            thrower.Interact();
            return;
        }

        LaunchInteractionProjectile();
    }

    private void LaunchInteractionProjectile()
    {
        if (!_projectilePool.TryGetProjectile(out var projectile))
            return;

        _isProjectileActive = true;
        Vector3 origin = _camera.transform.position;
        Vector3 direction = _camera.transform.forward;

        projectile.Launch(origin, direction, _projectileSpeed, OnProjectileHit);
    }

    private void OnProjectileHit(IInteractable interactable)
    {
        _isProjectileActive = false;

        if (interactable != null &&
            _targetDetector.IsValidTarget(_camera.transform.position, _camera.transform.forward, interactable.Transform.position))
        {
            interactable.Interact();
            return;
        }

        if (TryFindPlayerInteractable(out var fallback))
            fallback.Interact();
    }

    private bool TryFindPlayerInteractable(out IInteractable result)
    {
        result = null;
        foreach (var interactable in _playerInteractables)
        {
            if (interactable?.CanInteract == true)
            {
                result = interactable;
                return true;
            }
        }
        return false;
    }

    private bool TryFindPlayerInteractable<T>(out T result) where T : IInteractable
    {
        result = default;
        foreach (var interactable in _playerInteractables)
        {
            if (interactable is T typed && typed.CanInteract)
            {
                result = typed;
                return true;
            }
        }
        return false;
    }

    private void OnDrawGizmosSelected()
    {
        if (!_camera) return;

        Vector3 origin = _camera.transform.position;
        Vector3 forward = _camera.transform.forward;
        Vector3 left = Quaternion.AngleAxis(-_interactionAngle, Vector3.up) * forward;
        Vector3 right = Quaternion.AngleAxis(_interactionAngle, Vector3.up) * forward;

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(origin, left * _projectileSpeed);
        Gizmos.DrawRay(origin, right * _projectileSpeed);
        Gizmos.DrawRay(origin, forward * _projectileSpeed);
    }
}
