using Inventory.Structs;
using UnityEngine;
using Utilr.SoGameEvents;

namespace Inventory.SO
{
    [CreateAssetMenu(fileName = "ObtainItemEvent", menuName = "br/Inventory/ObtainItemEvent", order = 1)]
    public class ObtainItemEvent : SoCustomEvent<ItemCount>
    {
        
    }
}
