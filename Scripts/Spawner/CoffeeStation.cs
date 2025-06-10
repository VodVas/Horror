using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

public class CoffeeStation : MonoBehaviour
{
    [Header("Cup Positioning")]
    [SerializeField] private Transform _cupSnapPoint;
    [SerializeField] private float _snapSpeed = 5f;
    [SerializeField] private float _snapDistance = 0.5f;

    [Header("Coffee Machine")]
    [SerializeField] private CoffeeMachinesController _coffeeMachine;

    [Header("Visual Feedback")]
    [SerializeField] private GameObject _readyIndicator;
    [SerializeField] private GameObject _notReadyIndicator;

    private Cup _currentCup;
    private bool _isPositioning;
    private CancellationTokenSource _positioningCts;

    private void Awake()
    {
        if (_cupSnapPoint == null || _coffeeMachine == null || _readyIndicator == null || _notReadyIndicator == null)
        {
            Debug.LogError("Required components not assigned", this);
            enabled = false;
            return;
        }

        _coffeeMachine.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_currentCup != null || _isPositioning) return;

        if (other.TryGetComponent(out Cup cup) && !cup.HasCoffee && !cup.HasLid)
        {
            var distance = Vector3.Distance(cup.transform.position, _cupSnapPoint.position);

            if (distance <= _snapDistance)
            {
                StartCupPositioning(cup).Forget();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Cup cup) && cup == _currentCup)
        {
            if (!cup.IsInCoffeeMachine || cup.IsValidProduct())
            {
                RemoveCup();
            }
        }
    }

    private void OnDestroy()
    {
        _positioningCts?.Cancel();
        _positioningCts?.Dispose();
        UnsubscribeFromCupEvents();
    }

    private async UniTaskVoid StartCupPositioning(Cup cup)
    {
        _isPositioning = true;
        _currentCup = cup;

        _positioningCts?.Cancel();
        _positioningCts = new CancellationTokenSource();

        try
        {
            cup.transform.position = _cupSnapPoint.position;
            cup.transform.rotation = _cupSnapPoint.rotation;

            _currentCup.SetInCoffeeMachine(true);

            await UniTask.Yield(_positioningCts.Token);

            _coffeeMachine.enabled = true;
            SubscribeToCupEvents();
            UpdateReadyState();
        }
        catch (OperationCanceledException)
        {
            Debug.Log("Cup positioning was cancelled");
        }
        finally
        {
            _isPositioning = false;
        }
    }

    private void SubscribeToCupEvents()
    {
        if (_currentCup == null) return;

        _currentCup.OnProductStateChanged += OnCupProductStateChanged;
        _currentCup.OnInteractionRequested += OnCupInteractionRequested;
    }

    private void UnsubscribeFromCupEvents()
    {
        if (_currentCup == null) return;

        _currentCup.OnProductStateChanged -= OnCupProductStateChanged;
        _currentCup.OnInteractionRequested -= OnCupInteractionRequested;
    }

    private void OnCupProductStateChanged(bool isValid)
    {
        UpdateReadyState();

        if (isValid && _currentCup != null)
        {
            _currentCup.SetInCoffeeMachine(false);
        }
    }

    private void OnCupInteractionRequested(Food food)
    {
        if (_currentCup != null && _currentCup == food && _currentCup.IsValidProduct())
        {
            RemoveCup();
        }
    }

    private void UpdateReadyState()
    {
        bool isReady = _currentCup != null && _currentCup.IsValidProduct();

        _readyIndicator.SetActive(isReady);
        _notReadyIndicator.SetActive(!isReady);
    }

    private void RemoveCup()
    {
        UnsubscribeFromCupEvents();

        if (_currentCup != null)
        {
            _currentCup.SetInCoffeeMachine(false);
        }

        _currentCup = null;
        _coffeeMachine.enabled = false;

        _readyIndicator.SetActive(false);
        _notReadyIndicator.SetActive(true);
    }
}