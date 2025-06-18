using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

public interface ISceneLoader
{
    UniTask<float> LoadSceneAsync(string sceneName, LoadSceneMode mode, IProgress<float> progress, CancellationToken cancellationToken);
    UniTask<float> LoadSceneAsync(int sceneBuildIndex, LoadSceneMode mode, IProgress<float> progress, CancellationToken cancellationToken);
}
