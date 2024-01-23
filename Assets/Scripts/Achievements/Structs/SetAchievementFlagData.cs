using UnityEngine;

namespace Achievements.Structs
{
    [System.Serializable]
    public struct SetAchievementFlagData
    {
        [field: SerializeField]
        public string ApiName { get; set; }
    }
}
