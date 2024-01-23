using Achievements.Structs;
using UnityEngine;
using Utilr.SoGameEvents;

namespace Achievements.SO
{
    [CreateAssetMenu(fileName = "SetAchievementFlagEvent", menuName = "so/Achieve/SetAchievementFlagEvent", order = 1)]
    public class SetAchievementFlagEvent : SoCustomEvent<SetAchievementFlagData>
    {
        
    }
}
