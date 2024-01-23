using Dialogue.Struct;
using UnityEngine;
using Utilr.SoGameEvents;

namespace Dialogue.SO
{
    [CreateAssetMenu(fileName = "UpdateYarnVarValueEvent", menuName = "br/Dialogue/UpdateYarnVarValueEvent", order = 1)]
    public class YarnVariableUpdateEvent : SoCustomEvent<YarnVariableData>
    {
        
    }
}
