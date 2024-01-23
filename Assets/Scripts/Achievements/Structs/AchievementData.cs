using UnityEngine;

namespace Achievements.Structs
{
    [System.Serializable]
    public struct AchievementData
    {
        [field: SerializeField]
        public bool IsCompleted { get; set; }
        
        // TODO: add other fields related to type, hidden, icon, etc.
    }
}
