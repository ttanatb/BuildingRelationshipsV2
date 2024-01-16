using UnityEngine;
using Utilr.SoGameEvents;

namespace Dialogue.SO
{
    [CreateAssetMenu(fileName = "ChangeDialogueAudioEvent", menuName = "brDialogue/ChangeDialogueAudioEvent", order = 1)]
    public class ChangeDialogueAudioEvent : SoCustomEvent<AudioClip>
    {
        
    }
}
