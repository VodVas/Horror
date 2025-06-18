using UnityEngine;

public class TriggerFinishActivator : MonoBehaviour
{
    [SerializeField] private FadeEffect _fadeEffect;
    [SerializeField] private GameObject _surviveMessage;

    private void Awake()
    {
        if (_fadeEffect == null || _surviveMessage == null)
        {
            Debug.Log("FadeEffect not assigned", this);
            enabled = false;
            return;
        }
    }

    private void OnEnable()
    {
        _fadeEffect.OnFadeComplete += ShowMessage;
    }

    private void OnDisable()
    {
        _fadeEffect.OnFadeComplete -= ShowMessage;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Player _))
        {
            Time.timeScale = 0f;

            _fadeEffect.FadeIn();
        }
    }

    private void ShowMessage(bool isFadeIn)
    {
        if (isFadeIn)
        {
            _surviveMessage.SetActive(true);
        }
    }
}