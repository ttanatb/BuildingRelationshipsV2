using FlagSystem.SO;
using UnityEngine;

namespace FlagSystem.Structs
{
    /// <summary>
    /// Represents the path between the node (what is required to traverse the path).
    /// </summary>
    [System.Serializable]
    public struct FlagNodeConnection
    {
        [field: SerializeField] 
        public ItemPreReq ItemPreReq { get; set; }
        
        [field: SerializeField]
        public FlagNode Node { get; set; }
    }
}
