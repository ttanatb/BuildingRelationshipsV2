using UnityEngine;
using Utilr.SoGameEvents;

namespace Dialogue
{
    [CreateAssetMenu(fileName = "SetCurrentActorIdEvent", menuName = "br/Dialogue/SetCurrentActorIdEvent", order = 1)]
    public class SetCurrentActorIdEvent : SoCustomEvent<string>
    {
        
    }
}
