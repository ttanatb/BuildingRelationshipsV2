using UnityEngine;

namespace Inventory.Structs
{
    [System.Serializable]
    public struct ItemCount
    {
        [field: SerializeField]
        public ItemData.ItemID Id { get; set; }

        [field: SerializeField]
        public int Count { get; set; }
    }
}
