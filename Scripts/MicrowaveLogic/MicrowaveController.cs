//using Cysharp.Threading.Tasks;
//using System.Threading;
//using UnityEngine;

//[RequireComponent(typeof(AudioSource))]
//public sealed class MicrowaveController : MonoBehaviour
//{
//    [SerializeField] private float _cookingDuration = 5f;
//    [SerializeField] private MicrowaveDoorController _doorController;

//    private AudioSource _audioSource;
//    private CancellationTokenSource _cts;
//    private bool _isCooking;
//    private bool _isDoorClosed = true;

//    public bool IsCooking => _isCooking;

//    private void Awake() => _audioSource = GetComponent<AudioSource>();

//    private void OnDestroy()
//    {
//        _cts?.Cancel();
//        _cts?.Dispose();
//    }

//    public void NotifyDoorState(bool isClosed)
//    {
//        _isDoorClosed = isClosed;
//        if (!isClosed && _isCooking) CancelCooking();
//    }

//    public void StartCooking()
//    {
//        if (_isCooking || !_isDoorClosed) return;

//        _cts?.Cancel();
//        _cts = new CancellationTokenSource();
//        _audioSource.Play();
//        CookingProcess(_cts.Token).Forget();
//    }

//    private async UniTaskVoid CookingProcess(CancellationToken ct)
//    {
//        _isCooking = true;
//        try
//        {
//            await UniTask.Delay((int)(_cookingDuration * 1000),
//                cancellationToken: ct);

//            if (!ct.IsCancellationRequested)
//                _doorController.OpenDoor();
//        }
//        finally
//        {
//            _audioSource.Stop();
//            _isCooking = false;
//        }
//    }

//    private void CancelCooking()
//    {
//        _cts?.Cancel();
//        _cts?.Dispose();
//        _cts = null;
//    }
//}

using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public sealed class MicrowaveController : MonoBehaviour
{
    [SerializeField] private float _cookingDuration = 5f;
    [SerializeField] private MicrowaveDoorController _doorController;

    private AudioSource _audioSource;
    private CancellationTokenSource _cts;
    private bool _isCooking;
    private bool _isDoorClosed = true;

    public bool IsCooking => _isCooking;

    private void Awake() => _audioSource = GetComponent<AudioSource>();

    private void OnDestroy()
    {
        _cts?.Cancel();
        _cts?.Dispose();
    }

    public void NotifyDoorState(bool isClosed)
    {
        _isDoorClosed = isClosed;
        if (!isClosed && _isCooking) CancelCooking();
    }

    public void StartCooking()
    {
        if (_isCooking || !_isDoorClosed) return;

        _cts?.Cancel();
        _cts = new CancellationTokenSource();
        _audioSource.Play();
        CookingProcess(_cts.Token).Forget();
    }

    private async UniTaskVoid CookingProcess(CancellationToken ct)
    {
        _isCooking = true;
        try
        {
            await UniTask.Delay((int)(_cookingDuration * 1000), cancellationToken: ct);
            if (!ct.IsCancellationRequested) _doorController.OpenDoor();
        }
        finally
        {
            _audioSource.Stop();
            _isCooking = false;
        }
    }

    private void CancelCooking()
    {
        _cts?.Cancel();
        _cts?.Dispose();
        _cts = null;
    }
}
