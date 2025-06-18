using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ObjectActivator : MonoBehaviour 
{
    [SerializeField] private GameObject _targetObject;

    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
    }

    private void OnEnable()
    {
        _button.onClick.AddListener(ToggleVisibleObject);
    }

    private void OnDisable()
    {
        _button.onClick.RemoveListener(ToggleVisibleObject);
    }

    public void ToggleVisibleObject()
    {
        _targetObject.SetActive(!_targetObject.activeSelf);
    }
}