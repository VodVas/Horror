using UnityEngine;
using VodVas.InterfaceSerializer;

public sealed class InteractionManager : MonoBehaviour
{
    [SerializeField] private float _interactionDistance = 5f;
    [SerializeField] private float _sphereCastRadius = 0.3f;
    [SerializeField] private LayerMask _interactionLayer;
    [SerializeField] private Camera _playerCamera;
    [SerializeField, InterfaceConstraint(typeof(IInputProvider))]
    private MonoBehaviour _inputProvider;

    private RaycastHit[] _hits = new RaycastHit[8];
    private IInputProvider _input;
    private IInteractable _currentInteractable;

    private void Awake()
    {
        _input = _inputProvider as IInputProvider;
        if (_playerCamera == null) _playerCamera = Camera.main;
    }

    private void Update()
    {
        if (!_input.GetPushInput()) return;

        Ray ray = _playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        int hitCount = Physics.SphereCastNonAlloc(
            ray.origin,
            _sphereCastRadius,
            ray.direction,
            _hits,
            _interactionDistance,
            _interactionLayer,
            QueryTriggerInteraction.Collide
        );

        _currentInteractable = null;
        float minDistance = float.MaxValue;

        for (int i = 0; i < hitCount; i++)
        {
            if (_hits[i].distance < minDistance &&
                _hits[i].collider.TryGetComponent(out IInteractable interactable))
            {
                minDistance = _hits[i].distance;
                _currentInteractable = interactable;
            }
        }

        _currentInteractable?.Interact();
    }
}