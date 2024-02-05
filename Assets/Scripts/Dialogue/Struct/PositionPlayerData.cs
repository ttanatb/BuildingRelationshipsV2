using UnityEngine;

namespace Dialogue.Struct
{
    [System.Serializable]
    public struct PositionPlayerData
    {
        [field: SerializeField]
        public Transform Transform { get; set; }
        
        [field: SerializeField]
        public float Duration { get; set; }
    }
}
