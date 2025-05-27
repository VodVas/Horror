using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;

public sealed class AchievementManager : MonoBehaviour
{
    [SerializeField] private AchievementDisplay _display;
    [SerializeField] private AchievementData[] _achievements;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _achievementSound;

    private float _lastSoundTime;
    private readonly Dictionary<AchievementData, CancellationTokenSource> _activeCTS = new();
    private readonly Dictionary<AchievementData, int> _hitCounters = new();

    private void Awake()
    {
        foreach (var achievement in _achievements)
        {
            _hitCounters[achievement] = 0;
        }
    }

    public void ReportHit(AchievementData data)
    {
        if (!_hitCounters.ContainsKey(data)) return;

        if (++_hitCounters[data] >= data.RequiredHits)
        {
            _hitCounters[data] = 0;
            ProcessAchievement(data).Forget();
        }
    }

    private void PlayAchievementSound()
    {
        if (_audioSource == null || _achievementSound == null)
            return;

        if (Time.time - _lastSoundTime < 0.3f)
            return;

        _audioSource.Stop();
        _audioSource.PlayOneShot(_achievementSound);
        _lastSoundTime = Time.time;
    }

    private async UniTaskVoid ProcessAchievement(AchievementData data)
    {
        CancelActiveCTS(data);

        using var cts = new CancellationTokenSource();
        _activeCTS[data] = cts;

        try
        {
            PlayAchievementSound();
            await _display.ShowAchievement(data.DisplayName, data.Description, data.DisplayDuration, data.FadeDuration, cts.Token);
        }
        finally
        {
            _activeCTS.Remove(data);
        }
    }

    private void CancelActiveCTS(AchievementData data)
    {
        if (_activeCTS.TryGetValue(data, out var oldCts))
        {
            oldCts.Cancel();
            _activeCTS.Remove(data);
        }
    }
}