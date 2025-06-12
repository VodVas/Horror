using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class SceneTransitionManager : MonoBehaviour
{
    [SerializeField] private LoadingScreen _loadingScreenPrefab;
    [SerializeField] private float _minimumLoadingTime = 0.5f;

    private ISceneLoader _sceneLoader;
    private ILoadingScreen _loadingScreen;
    private CancellationTokenSource _lifetimeCts;
    private CancellationTokenSource _transitionCts;

    private void Awake()
    {
        _lifetimeCts = new CancellationTokenSource();
        _sceneLoader = GetComponent<ISceneLoader>() ?? gameObject.AddComponent<SceneLoader>();
        DontDestroyOnLoad(gameObject);
        InitializeLoadingScreen();
    }

    private void OnDestroy()
    {
        _transitionCts?.Cancel();
        _transitionCts?.Dispose();
        _lifetimeCts?.Cancel();
        _lifetimeCts?.Dispose();
    }

    private void InitializeLoadingScreen()
    {
        if (_loadingScreenPrefab != null)
        {
            var loadingScreenGO = Instantiate(_loadingScreenPrefab);
            DontDestroyOnLoad(loadingScreenGO);
            _loadingScreen = loadingScreenGO.GetComponent<ILoadingScreen>();
        }
    }

    public async UniTask TransitionToSceneAsync(string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
    {
        await TransitionInternalAsync(sceneName, mode);
    }

    public async UniTask TransitionToSceneAsync(int sceneBuildIndex, LoadSceneMode mode = LoadSceneMode.Single)
    {
        await TransitionInternalAsync(sceneBuildIndex, mode);
    }

    private async UniTask TransitionInternalAsync(object sceneReference, LoadSceneMode mode)
    {
        _transitionCts?.Cancel();
        _transitionCts?.Dispose();
        _transitionCts = new CancellationTokenSource();

        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(_lifetimeCts.Token, _transitionCts.Token);
        var token = linkedCts.Token;

        try
        {
            var showLoadingTask = _loadingScreen?.ShowAsync(token) ?? UniTask.CompletedTask;
            var minimumTimeTask = UniTask.Delay(TimeSpan.FromSeconds(_minimumLoadingTime), cancellationToken: token);

            await showLoadingTask;

            var progress = new Progress<float>(value => _loadingScreen?.UpdateProgress(value));

            var loadTask = sceneReference switch
            {
                string sceneName => _sceneLoader.LoadSceneAsync(sceneName, mode, progress, token),
                int sceneIndex => _sceneLoader.LoadSceneAsync(sceneIndex, mode, progress, token),
                _ => throw new ArgumentException("Invalid scene reference type")
            };

            await UniTask.WhenAll(loadTask, minimumTimeTask);

            await (_loadingScreen?.HideAsync(token) ?? UniTask.CompletedTask);
        }
        catch (OperationCanceledException)
        {
            Debug.Log("Scene transition was cancelled");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Scene transition failed: {ex.Message}");
            throw;
        }
    }
}