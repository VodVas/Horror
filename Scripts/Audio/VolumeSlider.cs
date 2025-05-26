using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class VolumeSlider : MonoBehaviour
{
    private const float DecibelsMultiplier = 20f;

    [SerializeField] private AudioMixerGroup _audioMixer;

    private Slider _slider;
    private float _minVolume = -80f;
    private float _maxVolume = 0f;
    private float _minValueLimit = 0.0001f;
    private float _maxValueLimit = 1f;

    private void Awake()
    {
        _slider = GetComponent<Slider>();
    }

    private void Start()
    {
        if (_audioMixer.audioMixer.GetFloat(_audioMixer.name, out float currentVolume))
        {
            _slider.value = Mathf.InverseLerp(_minVolume, _maxVolume, currentVolume);
        }
    }

    private void OnEnable()
    {
        _slider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    private void OnDisable()
    {
        _slider.onValueChanged.RemoveListener(OnSliderValueChanged);
    }

    private void OnSliderValueChanged(float value)
    {
        _audioMixer.audioMixer.SetFloat(_audioMixer.name, Mathf.Log10(Mathf.Clamp(value, _minValueLimit, _maxValueLimit)) * DecibelsMultiplier);
    }
}