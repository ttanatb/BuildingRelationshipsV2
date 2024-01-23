using UnityEngine;
using Inventory.SerializedDict;
using Inventory.SO;
using Inventory.Structs;
using Saves.SO;
using Saves.Structs;

public class InventorySystem : Singleton<InventorySystem>
{
    [SerializeField] private ItemIdToItemCountDict m_inventory = new ItemIdToItemCountDict();
    
    [SerializeField] private ObtainItemEvent m_obtainItemEvent = null;
    [SerializeField] private InventoryUpdatedEvent m_inventoryUpdatedEvent = null;
    [SerializeField] private LoadSaveEvent m_loadSaveEvent = null;

    private void Start()
    {
        m_obtainItemEvent.Event.AddListener(ObtainItemEvent);
        m_loadSaveEvent.Event.AddListener(OnSaveDataLoaded);
    }

    private void OnDestroy()
    {
        m_obtainItemEvent.Event.RemoveListener(ObtainItemEvent);
        m_loadSaveEvent.Event.RemoveListener(OnSaveDataLoaded);
    }

    private void OnSaveDataLoaded(SaveData data)
    {
        m_inventory.CopyFrom(data.Inventory);
        m_inventoryUpdatedEvent.Invoke(m_inventory);
    }

    private void ObtainItemEvent(ItemCount itemCount)
    {
        if (!m_inventory.TryGetValue(itemCount.Id, out var item))
        {
            m_inventory.Add(itemCount.Id, itemCount);
        }
        else
        {
            item.Count += itemCount.Count;
            m_inventory[itemCount.Id] = item;
        }

        m_inventoryUpdatedEvent.Invoke(m_inventory);
    }
}
