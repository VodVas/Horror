using UnityEngine;
using UnityEngine.UI;

public class MouseSensitivityController : MonoBehaviour
{
    [SerializeField] private MovementConfig _movementConfig;
    [SerializeField] private Slider _sensitivitySlider;

    private void Awake()
    {
        _sensitivitySlider.value = _movementConfig.MouseSensitivity;
        _sensitivitySlider.onValueChanged.AddListener(UpdateSensitivity);
    }

    private void OnDestroy()
    {
        _sensitivitySlider.onValueChanged.RemoveListener(UpdateSensitivity);
    }

    private void UpdateSensitivity(float value)
    {
        _movementConfig.SetMouseSensitivity(value);
    }
}