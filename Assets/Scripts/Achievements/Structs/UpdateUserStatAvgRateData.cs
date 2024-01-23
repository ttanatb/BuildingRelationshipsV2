using UnityEngine;

namespace Achievements.Structs
{
    [System.Serializable]
    public struct UpdateUserStatAvgRateData
    {
        [field: SerializeField]
        public string ApiName { get; set; }
        
        [field: SerializeField]
        public float Value { get; set; }

        [field: SerializeField]
        public float Duration { get; set; }
    }
}
