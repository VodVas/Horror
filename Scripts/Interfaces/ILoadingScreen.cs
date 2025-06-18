using System.Threading;
using Cysharp.Threading.Tasks;

public interface ILoadingScreen
{
    UniTask ShowAsync(CancellationToken cancellationToken);
    UniTask HideAsync(CancellationToken cancellationToken);
    void UpdateProgress(float progress);
}
