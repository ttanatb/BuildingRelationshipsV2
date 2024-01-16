using FlagSystem.SO;
using UnityEngine;

namespace FlagSystem.Structs
{
    [System.Serializable]
    public struct FlagGraphUpdateStatus
    {
        [field: SerializeField]
        public FlagNode[] CompletedNodes { get; set; }
        
        [field: SerializeField]
        public FlagNode[] CurrentNodes { get; set; }
        
        [field: SerializeField]
        public FlagNode[] NextNodes { get; set; }
    }
}
