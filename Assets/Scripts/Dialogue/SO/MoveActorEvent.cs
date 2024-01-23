using Dialogue.Struct;
using UnityEngine;
using Utilr.SoGameEvents;

namespace Dialogue.SO
{
    [CreateAssetMenu(fileName = "MoveActorEvent", menuName = "br/Dialogue/MoveActorEvent", order = 1)]
    public class MoveActorEvent : SoCustomEvent<MoveActorData>
    {
        
    }
}
