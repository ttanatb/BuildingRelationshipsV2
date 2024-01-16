using UnityEngine;
using Utilr.SoGameEvents;

namespace GameEvents
{
    [CreateAssetMenu(fileName = "SoToggleInstructionUi", menuName = "soEventsBr/SoToggleInstructionUi", order = 1)]
    public class SoSetUiActiveStateEvent : SoCustomEvent<bool>
    {
        
    }
}
