using UnityEngine;

public class LayerToggleTrigger : MonoBehaviour
{
    [SerializeField] private MicrowaveController _controller;
    [SerializeField] private LayerMask _interactionLayer;
    [SerializeField] private LayerMask _obstacleLayer;

    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent(out Food food))
        {
            if (_controller.IsDoorClosed)
                SetObstacleLayer(_obstacleLayer, food.gameObject);

            if (_controller.IsDoorOpen)
                SetObstacleLayer(_interactionLayer, food.gameObject);
        }
    }

    private void SetObstacleLayer(LayerMask layerMask, GameObject obj)
    {
        obj.layer = (int)Mathf.Log(layerMask.value, 2);
    }
}