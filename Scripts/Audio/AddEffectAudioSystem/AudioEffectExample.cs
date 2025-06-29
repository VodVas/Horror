using UnityEngine;

public class AudioEffectExample : MonoBehaviour
{
    [SerializeField] private AudioEffectProcessor _processor;
    [SerializeField] private AudioEffectPreset[] _presets;

    private void Start()
    {
        // Применяем первый пресет
        if (_presets.Length > 0)
        {
            _processor.ApplyPreset(_presets[0]);
        }
    }

    private void Update()
    {
        // Переключение пресетов клавишами 1-6
        for (int i = 0; i < Mathf.Min(_presets.Length, 6); i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                _processor.ApplyPreset(_presets[i]);
                Debug.Log($"Applied effect preset: {_presets[i].name}");
            }
        }

        // Bypass эффекта на Space
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _processor.SetBypass(!_processor.enabled);
        }

        // Регулировка wet/dry микса колесиком мыши
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            float currentMix = 1f; // Нужно добавить getter в processor
            _processor.SetWetDryMix(Mathf.Clamp01(currentMix + scroll * 0.1f));
        }
    }
}
