using Input.Structs;
using UnityEngine;
using Utilr.SoGameEvents;

namespace Input.SO
{
    [CreateAssetMenu(fileName = "TriggerGamepadRumbleEvent", menuName = "br/Input/TriggerGamepadRumbleEvent", order = 1)]
    public class TriggerGamepadRumbleEvent : SoCustomEvent<TriggerGamepadRumbleData>
    {
        
    }
}
