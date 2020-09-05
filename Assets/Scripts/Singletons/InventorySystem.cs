using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class InventorySystem : Singleton<InventorySystem>
{
    [SerializeField]
    private CollectibleSO m_collectibleSO = null;

#if UNITY_EDITOR
    const string PATH_PREFIX = "Assets/ScriptableObjects/";
    const string PATH_SUFFIX = ".asset";
    [SerializeField]
    string soAssetName;
#endif

    private SortedDictionary<CollectibleItem.ItemID, (int amt, int index)> m_inventory = null;
    [SerializeField]
    private List<(CollectibleItem.ItemID id, int amt)> m_sortedInventory = null;

    private Dictionary<CollectibleItem.ItemID, CollectibleItem> m_itemDatabase = null;
    private EventManager m_eventManager = null;

    public Dictionary<CollectibleItem.ItemID, CollectibleItem> ItemDatabase
    { get { return m_itemDatabase; } }

    public List<(CollectibleItem.ItemID, int)> Inventory { get { return m_sortedInventory; } }

    public UnityAction<List<(CollectibleItem.ItemID, int)>> ChangeItemAmtFinishedCb { get; set; }



    public void ChangeItemAmt(CollectibleItem.ItemID id, int amount)
    {
        if (!m_inventory.ContainsKey(id))
        {
            Debug.LogError("Item (" + id + ") doesn't exist");
            return;
        }

        int prevAmount = m_inventory[id].amt;
        int newAmt = amount == 0 ? 0 : Mathf.Max(m_inventory[id].amt + amount, 0);
        int newIndex;
        if (newAmt == 0)
        {
            if (m_inventory[id].index == -1)
            {
                Debug.LogError("Trying to remove item (" + id + ") even though it doesn't exist");
                return;
            }
            else if (m_inventory[id].index >= m_sortedInventory.Count)
            {
                Debug.LogError("Trying to remove item (" + id + ") at index (" +
                    m_inventory[id].index + ") which is out of bounds.");
                return;
            }

            m_sortedInventory.RemoveAt(m_inventory[id].index);
            newIndex = -1;
        }
        else
        {
            var item = (id, newAmt);
            if (prevAmount == 0)
            {
                newIndex = m_sortedInventory.Count;
                m_sortedInventory.Add(item);
            }
            else
            {
                m_sortedInventory[m_inventory[id].index] = item;
                newIndex = m_inventory[id].index;
            }
        }

        m_inventory[id] = (newAmt, newIndex);


        ChangeItemAmtFinishedCb?.Invoke(m_sortedInventory);
    }

    private void Awake()
    {
        m_itemDatabase = new Dictionary<CollectibleItem.ItemID, CollectibleItem>();
#if UNITY_EDITOR
        if (m_collectibleSO == null && soAssetName != "")
        {
            m_collectibleSO = (CollectibleSO)AssetDatabase.
                LoadAssetAtPath(PATH_PREFIX + soAssetName + PATH_SUFFIX, typeof(CollectibleSO));
        }
#endif
    }

    // Use this for initialization
    void Start()
    {
        m_inventory = new SortedDictionary<CollectibleItem.ItemID, (int amt, int index)>();
        m_eventManager = EventManager.Instance;

        foreach (var item in m_collectibleSO.items)
        {
            m_inventory.Add(item.id, (0, -1));
            m_itemDatabase.Add(item.id, item);
        }
        m_sortedInventory = new List<(CollectibleItem.ItemID, int)>(m_inventory.Count);

        m_eventManager.AddItemListener(ChangeItemAmt);
    }
}
