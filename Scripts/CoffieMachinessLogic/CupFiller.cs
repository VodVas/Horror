////using UnityEngine;

////public class CupFiller : MonoBehaviour
////{
////    [SerializeField] private LiquidVolumeAnimator _volumeAnimator;
////    [SerializeField] private float _targetLiquidLevel = 0.8f;
////    [SerializeField] private float _duration = 1f;

////    private bool _isFilling;

////    private void Awake()
////    {
////        if (_volumeAnimator == null)
////        {
////            Debug.Log("LiquidVolumeAnimator not assign", this);
////            enabled = false;
////            return;
////        }

////    }

////    private void OnParticleCollision(GameObject other)
////    {
////        Debug.Log("OnParticleCollision");
////        if (_isFilling) return;

////        _isFilling = true;
////        _volumeAnimator.AnimateFill(_targetLiquidLevel, _duration);
////    }
////}


//using UnityEngine;

//public class CupFiller : MonoBehaviour
//{
//    [SerializeField] private LiquidVolumeAnimator _volumeAnimator;
//    [SerializeField] private float _targetLiquidLevel = 0.8f;
//    [SerializeField] private float _duration = 1f;

//    private Cup _currentCup;
//    private bool _isFilling;

//    private void Awake()
//    {
//        if (_volumeAnimator == null)
//        {
//            Debug.LogError("LiquidVolumeAnimator not assigned", this);
//            enabled = false;
//            return;
//        }

//        _currentCup = GetComponentInParent<Cup>();
//        if (_currentCup == null)
//        {
//            Debug.LogError("Cup component not found in parent", this);
//            enabled = false;
//        }
//    }

//    private void OnParticleCollision(GameObject other)
//    {
//        if (_isFilling || _currentCup == null || _currentCup.HasCoffee) return;

//        _isFilling = true;
//        _volumeAnimator.AnimateFill(_targetLiquidLevel, _duration);
//        MarkCupAsFilled();
//    }

//    private void MarkCupAsFilled()
//    {
//        if (_currentCup != null)
//        {
//            _currentCup.MarkAsFilled();
//        }
//    }

//    private void OnDisable()
//    {
//        _isFilling = false;
//    }
//}


using UnityEngine;

public class CupFiller : MonoBehaviour
{
    [SerializeField] private LiquidVolumeAnimator _volumeAnimator;
    [SerializeField] private float _targetLiquidLevel = 0.75f;
    [SerializeField] private float _duration = 4f;

    private Cup _currentCup;
    private bool _isFilling;

    private void Awake()
    {
        _currentCup = GetComponentInParent<Cup>();

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
        _currentCup.SetCoffeeLevel(level);

        if (Mathf.Approximately(level, _targetLiquidLevel))
        {
            _currentCup.MarkAsFilled();
        }
    }
}