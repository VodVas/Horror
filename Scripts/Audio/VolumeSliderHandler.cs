using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
[RequireComponent(typeof(AudioSource))]
public class VolumeSliderHandler : MonoBehaviour
{
    private AudioSource effectAudioSource;

    private Slider _slider;
    private bool _isDragging;

    private void Awake()
    {
        _slider = GetComponent<Slider>();
        effectAudioSource = GetComponent<AudioSource>();

        SetupEventTriggers();
    }

    private void SetupEventTriggers()
    {
        var trigger = gameObject.AddComponent<EventTrigger>();

        var beginDragEntry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.BeginDrag
        };

        beginDragEntry.callback.AddListener(OnBeginDrag);
        trigger.triggers.Add(beginDragEntry);

        var endDragEntry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.EndDrag
        };

        endDragEntry.callback.AddListener(OnEndDrag);
        trigger.triggers.Add(endDragEntry);

        var pointerUpEntry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerUp
        };

        pointerUpEntry.callback.AddListener(OnEndDrag);
        trigger.triggers.Add(pointerUpEntry);
    }

    private void OnBeginDrag(BaseEventData data)
    {
        _isDragging = true;
        UpdateAudioState();
    }

    private void OnEndDrag(BaseEventData data)
    {
        _isDragging = false;
        UpdateAudioState();
    }

    private void OnEnable()
    {
        _slider.onValueChanged.AddListener(OnValueChanged);
    }

    private void OnDisable()
    {
        _slider.onValueChanged.RemoveListener(OnValueChanged);
        _isDragging = false;
        UpdateAudioState();
    }

    private void OnValueChanged(float value)
    {
        if (_isDragging)
        {
            effectAudioSource.volume = value;
        }
    }

    private void UpdateAudioState()
    {
        if (_isDragging)
        {
            effectAudioSource.volume = _slider.value;

            if (!effectAudioSource.isPlaying)
            {
                effectAudioSource.Play();
            }
        }
        else
        {
            if (effectAudioSource.isPlaying)
            {
                effectAudioSource.Stop();
            }
        }
    }
}