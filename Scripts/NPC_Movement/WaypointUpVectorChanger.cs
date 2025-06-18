using UnityEngine;

public class WaypointUpVectorChanger : MonoBehaviour
{
    [SerializeField] private Vector3 _newUpVector = Vector3.down;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out WaypointMover mover))
        {
            mover.SetCustomUpVector(_newUpVector);
        }
    }
}