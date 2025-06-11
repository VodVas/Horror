//using System;
//using UnityEngine;

//public class Cup : Food, IRewardableFood, IValidatable
//{
//    [SerializeField] private Transform _selfLid;
//    [SerializeField] private float _minCoffeeLevel = 0.1f;
//    [SerializeField] private LiquidVolumeAnimator _liquidAnimator;

//    private bool _hasCoffee;
//    private bool _hasLid;
//    private bool _isInCoffeeMachine;
//    private float _currentCoffeeLevel;

//    public override ItemType ItemType => ItemType.Cup;
//    public bool HasCoffee => _hasCoffee;
//    public bool HasLid => _hasLid;
//    public bool IsInCoffeeMachine => _isInCoffeeMachine;
//    public float CoffeeLevel => _currentCoffeeLevel;

//    public event Action<bool> OnCoffeeStateChanged;
//    public event Action<bool> OnLidStateChanged;
//    public event Action<bool> OnProductStateChanged;

//    protected override void Awake()
//    {
//        base.Awake();

//        if (_selfLid == null || _liquidAnimator == null)
//        {
//            Debug.LogError("Dependencies not assigned", this);
//            enabled = false;
//        }
//    }

//    private void OnCollisionEnter(Collision collision)
//    {
//        if (collision.gameObject.TryGetComponent(out Lid lid))
//        {
//            TryAttachLid(lid);
//        }
//    }

//    private void TryAttachLid(Lid lid)
//    {
//        if (_hasLid) return;

//        if (_isInCoffeeMachine && !_hasCoffee) return;

//        AttachLid();
//        lid.ReturnToPool();
//    }

//    private void AttachLid()
//    {
//        _hasLid = true;
//        _selfLid.gameObject.SetActive(true);
//        OnLidStateChanged?.Invoke(true);
//        OnProductStateChanged?.Invoke(IsValidProduct());
//    }

//    public void MarkAsFilled()
//    {
//        if (!_hasCoffee)
//        {
//            _hasCoffee = true;
//            _currentCoffeeLevel = 0.75f;
//            OnCoffeeStateChanged?.Invoke(true);
//            OnProductStateChanged?.Invoke(IsValidProduct());
//        }
//    }

//    public void SetCoffeeLevel(float level)
//    {
//        _currentCoffeeLevel = Mathf.Clamp01(level);

//        if (!_hasCoffee && _currentCoffeeLevel >= _minCoffeeLevel)
//        {
//            _hasCoffee = true;
//            OnCoffeeStateChanged?.Invoke(true);
//            OnProductStateChanged?.Invoke(IsValidProduct());
//        }
//    }

//    public void SetInCoffeeMachine(bool inMachine)
//    {
//        _isInCoffeeMachine = inMachine;
//    }

//    public bool IsRewardable()
//    {
//        return IsValidProduct();
//    }

//    public bool IsValidProduct()
//    {
//        return _hasCoffee && _hasLid && _currentCoffeeLevel >= _minCoffeeLevel;
//    }

//    public override bool CanInteract => base.CanInteract && (!_isInCoffeeMachine || IsValidProduct());

//    public override void Interact()
//    {
//        if (!CanInteract) return;

//        RaiseInteractionRequested();

//        if (_isInCoffeeMachine && IsValidProduct())
//        {
//            return;
//        }

//        base.Interact();
//    }

//    protected override void ResetSpecificState()
//    {
//        _selfLid.gameObject.SetActive(false);
//        _hasCoffee = false;
//        _hasLid = false;
//        _isInCoffeeMachine = false;
//        _currentCoffeeLevel = 0f;

//        _liquidAnimator.ResetToEmpty();
//        OnCoffeeStateChanged?.Invoke(false);
//        OnLidStateChanged?.Invoke(false);
//        OnProductStateChanged?.Invoke(false);
//    }

//    protected override void OnConfigureInHand()
//    {
//        transform.localRotation = Quaternion.identity;
//        _isInCoffeeMachine = false;
//    }
//}



using System;
using UnityEngine;

public class Cup : Food, IRewardableFood
{
    [SerializeField] private Transform _selfLid;
    [SerializeField] private float _minCoffeeLevel = 0.75f;
    [SerializeField] private LiquidVolumeAnimator _liquidAnimator;

    private bool _hasCoffee;
    private bool _hasLid;
    private bool _isInCoffeeMachine;
    private float _currentCoffeeLevel;

    public override ItemType ItemType => ItemType.Cup;
    public bool HasCoffee => _hasCoffee;
    public bool HasLid => _hasLid;
    public bool IsInCoffeeMachine => _isInCoffeeMachine;
    public float CoffeeLevel => _currentCoffeeLevel;

    public event Action<bool> OnCoffeeStateChanged;
    public event Action<bool> OnLidStateChanged;
    public event Action<bool> OnProductStateChanged;

    protected override void Awake()
    {
        base.Awake();

        if (_selfLid == null || _liquidAnimator == null)
        {
            Debug.LogError("Dependencies not assigned", this);
            enabled = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out Lid lid))
        {
            TryAttachLid(lid);
        }
    }

    private void TryAttachLid(Lid lid)
    {
        if (_hasLid) return;

        if (_isInCoffeeMachine && !_hasCoffee) return;

        AttachLid();
        lid.ReturnToPool();
    }

    private void AttachLid()
    {
        _hasLid = true;
        _selfLid.gameObject.SetActive(true);
        OnLidStateChanged?.Invoke(true);
        OnProductStateChanged?.Invoke(IsRewardable());
    }

    public void MarkAsFilled()
    {
        if (!_hasCoffee)
        {
            Debug.Log("MarkAsFilled()");
            _hasCoffee = true;
            _currentCoffeeLevel = 0.75f;
            OnCoffeeStateChanged?.Invoke(true);
            OnProductStateChanged?.Invoke(IsRewardable());
        }
    }

    public void SetCoffeeLevel(float level)
    {
        _currentCoffeeLevel = Mathf.Clamp01(level);

        if (!_hasCoffee && _currentCoffeeLevel >= _minCoffeeLevel)
        {
            _hasCoffee = true;
            OnCoffeeStateChanged?.Invoke(true);
            OnProductStateChanged?.Invoke(IsRewardable());
        }
    }

    public void SetInCoffeeMachine(bool inMachine)
    {
        _isInCoffeeMachine = inMachine;
    }

    public bool IsRewardable()
    {
        return _hasCoffee && _hasLid && _currentCoffeeLevel >= _minCoffeeLevel;
    }

    public override bool CanInteract => base.CanInteract && (!_isInCoffeeMachine || IsRewardable());

    public override void Interact()
    {
        if (!CanInteract) return;

        RaiseInteractionRequested();

        if (_isInCoffeeMachine && IsRewardable())
        {
            return;
        }

        base.Interact();
    }

    protected override void ResetSpecificState()
    {
        _selfLid.gameObject.SetActive(false);
        _hasCoffee = false;
        _hasLid = false;
        _isInCoffeeMachine = false;
        _currentCoffeeLevel = 0f;

        _liquidAnimator.ResetToEmpty();
        OnCoffeeStateChanged?.Invoke(false);
        OnLidStateChanged?.Invoke(false);
        OnProductStateChanged?.Invoke(false);
    }

    protected override void OnConfigureInHand()
    {
        transform.localRotation = Quaternion.identity;
        _isInCoffeeMachine = false;
    }
}