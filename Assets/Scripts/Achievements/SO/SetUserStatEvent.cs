using Achievements.Structs;
using UnityEngine;
using Utilr.SoGameEvents;

namespace Achievements.SO
{
    [CreateAssetMenu(fileName = "SetUserStatEvent", menuName = "so/Achieve/SetUserStatEvent", order = 1)]
    public class SetUserStatEvent : SoCustomEvent<SetUserStatData>
    {
        
    }
}
