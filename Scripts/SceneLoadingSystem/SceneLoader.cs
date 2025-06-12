using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class SceneLoader : MonoBehaviour, ISceneLoader
{
    private CancellationTokenSource _lifetimeCts;
    private CancellationTokenSource _operationCts;

    private void Awake()
    {
        _lifetimeCts = new CancellationTokenSource();
    }

    private void OnDestroy()
    {
        _operationCts?.Cancel();
        _operationCts?.Dispose();
        _lifetimeCts?.Cancel();
        _lifetimeCts?.Dispose();
    }

    public async UniTask<float> LoadSceneAsync(string sceneName, LoadSceneMode mode, IProgress<float> progress, CancellationToken cancellationToken)
    {
        return await LoadSceneInternalAsync(sceneName, mode, progress, cancellationToken);
    }

    public async UniTask<float> LoadSceneAsync(int sceneBuildIndex, LoadSceneMode mode, IProgress<float> progress, CancellationToken cancellationToken)
    {
        return await LoadSceneInternalAsync(sceneBuildIndex, mode, progress, cancellationToken);
    }

    private async UniTask<float> LoadSceneInternalAsync(object sceneReference, LoadSceneMode mode, IProgress<float> progress, CancellationToken externalToken)
    {
        _operationCts?.Cancel();
        _operationCts?.Dispose();
        _operationCts = new CancellationTokenSource();

        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(_lifetimeCts.Token, _operationCts.Token, externalToken);
        var token = linkedCts.Token;

        try
        {
            var operation = sceneReference switch
            {
                string sceneName => SceneManager.LoadSceneAsync(sceneName, mode),
                int sceneIndex => SceneManager.LoadSceneAsync(sceneIndex, mode),
                _ => throw new ArgumentException("Invalid scene reference type")
            };

            if (operation == null)
                throw new InvalidOperationException($"Failed to start loading scene: {sceneReference}");

            operation.allowSceneActivation = false;
            var startTime = Time.realtimeSinceStartup;

            while (!operation.isDone)
            {
                token.ThrowIfCancellationRequested();

                var currentProgress = operation.progress < 0.9f ? operation.progress : 1f;
                progress?.Report(currentProgress);

                if (operation.progress >= 0.9f)
                {
                    operation.allowSceneActivation = true;
                }

                await UniTask.Yield(PlayerLoopTiming.Update, token);
            }

            var loadTime = Time.realtimeSinceStartup - startTime;
            progress?.Report(1f);
            return loadTime;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Scene loading failed: {ex.Message}");
            throw;
        }
    }
}