using UnityEngine;

public class EmoSetter : MonoBehaviour
{
    [SerializeField] private EmotionType _enterEmotion = EmotionType.Happy;
    [SerializeField] private FacialExpressionController _expressionController;
    [SerializeField] private float _duration = 0.5f;
    [SerializeField] private float _intensity = 1.0f;

    private void Awake()
    {
        if (_expressionController == null)
        {
            Debug.Log("FacialExpressionController not assign", this);
            enabled = false;
            return;
        }
    }

    public void SetEnterEmotion() => SetEmotion(_enterEmotion);
    public void SetExitEmotion() => SetNeutralEmotion();

    public void SetNeutralEmotion()
    {
        _expressionController.ResetToNeutral();
    }

    private void SetEmotion(EmotionType emotion)
    {
        _expressionController.SetEmotion(emotion, _intensity, _duration);
    }
}