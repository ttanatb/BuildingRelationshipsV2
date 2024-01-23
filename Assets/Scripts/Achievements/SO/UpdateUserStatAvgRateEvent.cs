using Achievements.Structs;
using UnityEngine;
using Utilr.SoGameEvents;

namespace Achievements.SO
{
    [CreateAssetMenu(fileName = "UpdateUserStatAvgRateEvent", menuName = "so/Achieve/UpdateUserStatAvgRateEvent", order = 1)]
    public class UpdateUserStatAvgRateEvent : SoCustomEvent<UpdateUserStatAvgRateData>
    {
        
    }
}
