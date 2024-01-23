using RichPresence.Structs;
using UnityEngine;
using Utilr.SoGameEvents;

namespace RichPresence.SO
{
    [CreateAssetMenu(fileName = "SetRichPresenceEvent", menuName = "so/RichPresence/SetRichPresenceEvent", order = 1)]
    public class SetRichPresenceEvent : SoCustomEvent<RichPresenceData>
    {
        
    }
}
