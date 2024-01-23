using UnityEngine;
using UnityEngine.InputSystem;
using Utilr.SoGameEvents;

namespace Input.SO
{
    [CreateAssetMenu(fileName = "SwitchInputActionMapEvent", menuName = "br/Input/SwitchInputActionMapEvent", order = 1)]
    public class SwitchInputActionMapEvent : SoCustomEvent<InputActionReference>
    {
        
    }
}
