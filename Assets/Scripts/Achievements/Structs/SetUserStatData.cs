using UnityEngine;

namespace Achievements.Structs
{
    [System.Serializable]
    public struct SetUserStatData
    {
        [field: SerializeField]
        public string ApiName { get; set; }
        
        [field: SerializeField]
        public float Value { get; set; }
    }
}
