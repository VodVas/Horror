//using UnityEngine;

//public class NPCInteractionHeadHandler : NPCInteractionHandler
//{
//    [SerializeField] private RandomMusicPlayer _headSoundPlayer;
//    [SerializeField] private FacialEmotionPreset _headEmotionPreset;

//#if UNITY_EDITOR
//    private void OnValidate()
//    {
//        if (_headEmotionPreset == null || _headSoundPlayer == null)
//        {
//            Debug.Log("Dependencies not assigned", this);
//            enabled = false;
//            return;
//        }
//    }
//#endif

//    protected override void PlaySpecificSounds() => _headSoundPlayer.Play();
//    protected override void HandleInteraction() => PlayInteraction(_headEmotionPreset);
//}

using UnityEngine;

public class NPCInteractionHeadHandler : NPCInteractionHandler
{
}