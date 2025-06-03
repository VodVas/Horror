using UnityEngine;

public class EmoSetter : MonoBehaviour
{
    [SerializeField] private EmotionType _enterEmotion = EmotionType.Happy;
    //[SerializeField] private EmotionType _exitEmotion = EmotionType.Neutral;
    [SerializeField] private FacialExpressionController _expressionController;

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
        _expressionController.SetEmotion(emotion);
    }
}