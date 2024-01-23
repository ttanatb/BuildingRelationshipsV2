using Inventory.Structs;

namespace Inventory.SerializedDict
{
    [System.Serializable]
    public class ItemIdToItemCountDict : SerializableDictionary<ItemData.ItemID, ItemCount>
    {
        
    }
}
