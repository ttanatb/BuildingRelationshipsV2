using Inventory.Structs;
using Skills.Structs;

namespace Skills.SerializedDict
{
    [System.Serializable]
    public class ItemIdToSkillTypeDict : SerializableDictionary<ItemData.ItemID, PlayerSkill.SkillType>
    {
        
    }
}
