using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MicrowaveController : MonoBehaviour
{
    private const int Multiplier = 1000;

    [SerializeField] private DoorController doorController;
    [SerializeField, Range(0.1f, 10f)] private float _cookingDurationSeconds = 5f;

    private AudioSource _workSound;
    private bool _isCooking;
    private bool _isDoorClosed = true;
    private CancellationTokenSource _cookingCts;

    public bool IsCooking => _isCooking;

    private void Awake() => _workSound = GetComponent<AudioSource>();
    private void OnDestroy() => _cookingCts?.Cancel();

    public void NotifyDoorState(bool isClosed)
    {
        _isDoorClosed = isClosed;
        if (!isClosed && _isCooking) CancelCooking();
    }

    public void StartCooking()
    {
        if (_isCooking || !_isDoorClosed) return;
        _cookingCts?.Cancel();
        _cookingCts = new CancellationTokenSource();
        _workSound.Play();
        CookingProcess(_cookingCts.Token).Forget();
    }

    private async UniTaskVoid CookingProcess(CancellationToken ct)
    {
        _isCooking = true;
        try
        {
            await UniTask.Delay((int)(_cookingDurationSeconds * Multiplier), cancellationToken: ct);
            if (!ct.IsCancellationRequested) doorController.OpenDoor();
        }
        finally
        {
            _workSound.Stop();
            _isCooking = false;
        }
    }

    private void CancelCooking()
    {
        _cookingCts?.Cancel();
        _cookingCts?.Dispose();
        _cookingCts = null;
    }
}