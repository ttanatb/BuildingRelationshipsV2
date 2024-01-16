using UnityEngine;

namespace FlagSystem.Structs
{
    /// <summary>
    /// The required amount to be able to move on to another node.
    /// </summary>
    [System.Serializable]
    public struct ItemPreReq
    {
        [field: SerializeField]
        public CollectibleItem.ItemID ItemID { get; set; }
        
        [field: SerializeField]
        public int Amount { get; set; }
    }
}
