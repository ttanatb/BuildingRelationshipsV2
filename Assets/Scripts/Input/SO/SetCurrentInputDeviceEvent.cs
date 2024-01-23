using Input.Structs;
using UnityEngine;
using Utilr.SoGameEvents;

namespace Input.SO
{
    [CreateAssetMenu(fileName = "SetCurrentInputDeviceEvent", menuName = "br/Input/SetCurrentInputDeviceEvent", order = 1)]
    public class SetCurrentInputDeviceEvent : SoCustomEvent<SetCurrentInputDeviceData>
    {
        
    }
}
