using UnityEngine;
using VodVas.InterfaceSerializer;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private float _interactionDistance = 2f;
    [SerializeField] private float _sphereCastRadius = 0.1f;
    [SerializeField] private LayerMask _interactionLayer;
    [SerializeField, InterfaceConstraint(typeof(IInputProvider))]
    private MonoBehaviour _inputProvider;

    private Camera _cachedCamera;
    private RaycastHit[] _hits = new RaycastHit[2];
    private IInputProvider InputProvider => _inputProvider as IInputProvider;

    private void Start() => _cachedCamera = Camera.main;

    private void Update()
    {
        if (!InputProvider.GetPushInput()) return;
        if (FindNearestInteractable(out IInteractable interactable))
            interactable.Interact();
    }

    private bool FindNearestInteractable(out IInteractable result)
    {
        result = null;
        Ray ray = _cachedCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        int hitsCount = Physics.SphereCastNonAlloc(
            ray.origin,
            _sphereCastRadius,
            ray.direction,
            _hits,
            _interactionDistance,
            _interactionLayer
        );

        float minDistance = float.MaxValue;
        for (int i = 0; i < hitsCount; i++)
        {
            if (_hits[i].collider.TryGetComponent(out IInteractable interactable) &&
                _hits[i].distance < minDistance)
            {
                minDistance = _hits[i].distance;
                result = interactable;
            }
        }
        return result != null;
    }

    private void OnDrawGizmosSelected()
    {
        if (!Camera.main) return;
        Gizmos.color = Color.cyan;
        Vector3 position = Camera.main.transform.position;
        Vector3 direction = Camera.main.transform.forward * _interactionDistance;
        Gizmos.DrawWireSphere(position + direction, _sphereCastRadius);
    }
}