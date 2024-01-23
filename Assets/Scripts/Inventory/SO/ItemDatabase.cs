using Inventory.SerializedDict;
using UnityEngine;

namespace Inventory.SO
{
    [CreateAssetMenu(fileName = "ItemDatabase", menuName = "br/Inventory/ItemDatabase", order = 1)]
    public class ItemDatabase : ScriptableObject
    {
        [field: SerializeField]
        public ItemIdToItemDataDict Dict { get; set; }
    }
}
