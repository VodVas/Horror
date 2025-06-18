using UnityEngine;

public class WaypointOrientationModeChanger : MonoBehaviour
{
    [SerializeField] private WaypointMover.OrientationMode _orientationMode = WaypointMover.OrientationMode.LookAtDirection;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out WaypointMover mover))
        {
            mover.SetOrientationMode(_orientationMode);
        }
    }
}