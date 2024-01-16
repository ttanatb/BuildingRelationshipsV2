using FlagSystem.SO;
using UnityEngine;

namespace FlagSystem.Structs
{
    /// <summary>
    /// Status of how many items (of the required amount) is available. Used to convey information to the UI.
    /// </summary>
    [System.Serializable]
    public struct FlagItemPreReqUpdateStatus
    {
        [field: SerializeField]
        public FlagNode StartNode { get; set; }

        [field: SerializeField]
        public ItemPreReq ItemPreReq { get; set; }
        
        [field: SerializeField]
        public ItemPreReq CurrentAmount { get; set; }

        [field: SerializeField]
        public FlagNode EndNode { get; set; }
    }
}
