using UnityEngine;

public class NPCInteractionBodyHandler : NPCInteractionHandler
{
    [SerializeField] private RandomMusicPlayer _bodySoundPlayer;
    [SerializeField] private FacialEmotionPreset _bodyEmotionPreset;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_bodyEmotionPreset == null || _bodySoundPlayer == null)
        { 
            Debug.Log("Dependencies not assigned", this);
            enabled = false;
            return;
        }
    }
#endif

    protected override void PlaySpecificSounds() => _bodySoundPlayer.Play();
    protected override void HandleInteraction() => PlayInteraction(_bodyEmotionPreset);
}