using Inventory.SerializedDict;
using UnityEngine;
using Utilr.SoGameEvents;

namespace Inventory.SO
{
    [CreateAssetMenu(fileName = "InventoryUpdatedEvent", menuName = "br/Inventory/InventoryUpdatedEvent", order = 1)]
    public class InventoryUpdatedEvent : SoCustomEvent<ItemIdToItemCountDict>
    {
        
    }
}
