using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Button))]
public sealed class SceneLoaderButton : MonoBehaviour, IDisposable
{
    [Header("Dependencies")]
    [SerializeField] private SceneTransitionManager _transitionManager;
    [SerializeField] private Button _button;

    [Header("Settings")]
    [SerializeField] private int _sceneIndex = 1;
    [SerializeField] private LoadSceneMode _mode = LoadSceneMode.Single;

    private CancellationTokenSource _cts;
    private bool _isSubscribed;

    private void Awake()
    {
        _button = _button ? _button : GetComponent<Button>();
        _transitionManager = _transitionManager ? _transitionManager : FindObjectOfType<SceneTransitionManager>();
    }

    private void OnEnable() => Subscribe();
    private void OnDisable() => Unsubscribe();

    public void Dispose()
    {
        Unsubscribe();
        CancelLoading();
    }

    private void Subscribe()
    {
        if (_isSubscribed || _button == null) return;

        _button.onClick.AddListener(StartLoading);
        _isSubscribed = true;
    }

    private void Unsubscribe()
    {
        if (!_isSubscribed || _button == null) return;

        _button.onClick.RemoveListener(StartLoading);
        _isSubscribed = false;
    }

    private void StartLoading()
    {
        CancelLoading();
        LoadSceneAsync().Forget();
    }

    private async UniTaskVoid LoadSceneAsync()
    {
        _cts = new CancellationTokenSource();

        try
        {
            if (_transitionManager != null)
            {
                await _transitionManager.TransitionToSceneAsync(
                    _sceneIndex,
                    _mode)
                    .AttachExternalCancellation(_cts.Token);
            }
            else
            {
                await SceneManager.LoadSceneAsync(
                    _sceneIndex,
                    _mode)
                    .ToUniTask()
                    .AttachExternalCancellation(_cts.Token);
            }
        }
        catch (OperationCanceledException)
        {
            Debug.Log("Scene loading cancelled");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Scene loading failed: {ex}");
        }
        finally
        {
            _cts?.Dispose();
            _cts = null;
        }
    }

    private void CancelLoading()
    {
        _cts?.Cancel();
        _cts?.Dispose();
        _cts = null;
    }
}