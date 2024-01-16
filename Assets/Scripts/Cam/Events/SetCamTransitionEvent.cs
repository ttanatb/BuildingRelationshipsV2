using Cam.Struct;
using UnityEngine;
using Utilr.SoGameEvents;

namespace Cam.Events
{
    [CreateAssetMenu(fileName = "SetCamTransitionEvent", menuName = "brDialogue/SetCamTransitionEvent", order = 1)]
    public class SetCamTransitionEvent : SoCustomEvent<CamTransitionData>
    {
        
    }
}
