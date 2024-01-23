using Inventory.Structs;
using UnityEngine;

namespace Fishing.Structs
{
    [System.Serializable]
    public struct FishReelEndData
    {
        [field: SerializeField]
        public bool Success { get; set; }
		
        [field: SerializeField]
        public ItemData.ItemID Id { get; set; }
		
        [field: SerializeField]
        public Fish Fish { get; set; }
    }
}
