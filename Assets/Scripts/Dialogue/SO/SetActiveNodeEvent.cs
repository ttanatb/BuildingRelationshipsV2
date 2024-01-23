using Dialogue.Struct;
using UnityEngine;
using Utilr.SoGameEvents;

namespace Dialogue.SO
{
    [CreateAssetMenu(fileName = "SetActiveNodeEvent", menuName = "br/Dialogue/SetActiveNodeEvent", order = 1)]
    public class SetActiveNodeEvent : SoCustomEvent<SetActiveNodeData>
    {
        
    }
}
