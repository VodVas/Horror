using UnityEngine;

public sealed class OptimizedFootstepController : IFootstepController
{
    private readonly AudioSource _source;
    private readonly MovementConfig _config;
    private float _stepTimer;
    private int _lastIndex;

    public OptimizedFootstepController(AudioSource source, MovementConfig config)
    {
        _source = source;
        _config = config;
    }

    public void UpdateFootsteps(Vector2 input, MovementState state, float deltaTime)
    {
        if (!IsEligible(state, input)) return;

        _stepTimer += deltaTime;
        float interval = GetInterval(state);

        if (_stepTimer < interval) return;

        PlaySound();
        _stepTimer = 0;
    }

    private bool IsEligible(MovementState state, Vector2 input)
    {
        bool isGrounded = (state & MovementState.Grounded) != 0;
        bool isMoving = input.sqrMagnitude > 0.01f;
        return isGrounded && isMoving && _config.FootstepClips?.Length > 0;
    }

    private float GetInterval(MovementState state) =>
        (state & MovementState.Sprinting) != 0 ?
        _config.SprintStepInterval :
        _config.WalkStepInterval;

    private void PlaySound()
    {
        int index = GetRandomIndex();
        _source.pitch = Random.Range(_config.PitchRange.x, _config.PitchRange.y);
        _source.PlayOneShot(_config.FootstepClips[index], _config.FootstepVolume);
    }

    private int GetRandomIndex()
    {
        if (_config.FootstepClips.Length == 1) return 0;

        int index;
        do
        {
            index = Random.Range(0, _config.FootstepClips.Length);
        } while (index == _lastIndex);

        _lastIndex = index;
        return index;
    }
}