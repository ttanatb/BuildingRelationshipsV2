using Fishing.Structs;
using Inventory.Structs;

namespace Fishing.SerializedDict
{
    [System.Serializable]
    public class ItemIdToFishDataDict : SerializableDictionary<ItemData.ItemID, FishData>
    {
        
    }
}
