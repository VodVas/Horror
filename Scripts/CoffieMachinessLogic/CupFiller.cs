using UnityEngine;

public class CupFiller : MonoBehaviour
{
    [SerializeField] private LiquidVolumeAnimator _volumeAnimator;
    [SerializeField] private float _targetLiquidLevel = 0.75f;
    [SerializeField] private float _duration = 4f;
    [SerializeField] private Cup _currentCup;

    private bool _isFilling;

    private void Awake()
    {
        if (_volumeAnimator == null || _currentCup == null)
        {
            Debug.LogError("Required components not assigned", this);
            enabled = false;
        }
    }

    private void OnEnable() =>
        _volumeAnimator.OnLevelChanged += HandleLevelChanged;

    private void OnDisable()
    {
        _volumeAnimator.OnLevelChanged -= HandleLevelChanged;
        _isFilling = false;
    }

    private void OnParticleCollision(GameObject other)
    {
        if (_isFilling || _currentCup.HasCoffee) return;

        _isFilling = true;
        _volumeAnimator.AnimateFill(_targetLiquidLevel, _duration);
    }

    private void HandleLevelChanged(float level)
    {
        Debug.Log("HandleLevelChanged(float level)");
        _currentCup.SetCoffeeLevel(level);

        if (Mathf.Approximately(level, _targetLiquidLevel))
        {
            _currentCup.MarkAsFilled();
        }
    }
}