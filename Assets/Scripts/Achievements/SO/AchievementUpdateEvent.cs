using Achievements.Structs;
using UnityEngine;
using Utilr.SoGameEvents;

namespace Achievements.SO
{
    [CreateAssetMenu(fileName = "AchievementDataUpdateEvent", menuName = "so/Achieve/AchievementDataUpdateEvent", order = 1)]
    public class AchievementUpdateEvent : SoCustomEvent<AchievementCompletionData>
    {
        
    }
}
