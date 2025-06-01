using UnityEngine;

public class CupFiller : MonoBehaviour
{
    [SerializeField] private LiquidVolumeAnimator _volumeAnimator;
    [SerializeField] private float _targetLiquidLevel = 0.8f;
    [SerializeField] private float _duration = 1f;

    private bool _isFilling;

    private void Awake()
    {
        if (_volumeAnimator == null)
        {
            Debug.Log("LiquidVolumeAnimator not assign", this);
            enabled = false;
            return;
        }
            
    }

    private void OnParticleCollision(GameObject other)
    {
        if (_isFilling) return;

        _isFilling = true;
        _volumeAnimator.AnimateFill(_targetLiquidLevel, _duration);
    }
}