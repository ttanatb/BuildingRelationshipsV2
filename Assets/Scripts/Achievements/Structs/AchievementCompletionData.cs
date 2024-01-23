using Achievements.SerializedDict;
using UnityEngine;

namespace Achievements.Structs
{
    [System.Serializable]
    public struct AchievementCompletionData
    {
        [field: SerializeField]
        public StringToAchievementDict AchievementDict { get; set; }
    }
}
