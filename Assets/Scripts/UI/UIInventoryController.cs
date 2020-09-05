using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIInventoryController : MonoBehaviour
{
    [SerializeField]
    UIInventorySlotView[] m_inventoryslots = null;

    [SerializeField]
    Button m_pageLeftButton = null;

    [SerializeField]
    Button m_pageRightButton = null;

    RectTransform m_rectTransform = null;

    InventorySystem m_inventorySystem = null;

    private Dictionary<CollectibleItem.ItemID, CollectibleItem> m_itemDatabase = null;
    private List<(CollectibleItem.ItemID id, int amt)> m_sortedInventory = null;

    private bool m_isCurrentlyShowing = false;

    private Vector2 m_anchoredPos = Vector2.zero;

    [SerializeField]
    private Vector2 m_outOfScreenPos = Vector2.down * 1000;


    private int m_lastSelectedIndex = 0;
    private int m_pageIndex = 0;

    private void UpdateActiveSlots()
    {
        int slotIndex = 0;
        for (int i = m_pageIndex * m_inventoryslots.Length;
            i < m_sortedInventory.Count; i++)
        {
            if (i >= m_sortedInventory.Count)
            {
                if (slotIndex == 0 && m_pageIndex != 0)
                {
                    Debug.LogWarning("Empty second page?");
                }
                break;
            }

            var (id, amount) = m_sortedInventory[i];
            if (amount < 1)
                continue;

            if (slotIndex >= m_inventoryslots.Length)
                break;

            m_inventoryslots[slotIndex].Collectible = m_itemDatabase[id];
            m_inventoryslots[slotIndex].Amount = amount;
            slotIndex++;
        }

        for (int i = slotIndex; i < m_inventoryslots.Length; i++)
        {
            m_inventoryslots[i].Collectible = new CollectibleItem();
            m_inventoryslots[i].Amount = 0;
        }
    }

    private void UpdatePaginationButtons()
    {
        int totalPages = Mathf.CeilToInt(m_sortedInventory.Count / (float)m_inventoryslots.Length);
        if (m_pageIndex >= totalPages)
        {
            m_pageIndex = Mathf.Max(0, totalPages - 1);
        }

        m_pageLeftButton.interactable = m_pageIndex > 0;
        m_pageRightButton.interactable = m_pageIndex < totalPages - 1;
    }

    private void UpdateDisplay()
    {
        UpdatePaginationButtons();
        UpdateActiveSlots();
    }

    public void PageLeft()
    {
        m_pageIndex--;

        if (m_pageIndex < 0)
        {
            Debug.LogError("Somehow managed to set page index to " + m_pageIndex);
            m_pageIndex = 0;
        }
        UpdateDisplay();
        m_pageRightButton.Select();
    }

    public void PageRight()
    {
        m_pageIndex++;

        int totalPages = Mathf.CeilToInt(m_sortedInventory.Count / (float)m_inventoryslots.Length);
        if (m_pageIndex >= totalPages)
        {
            Debug.LogError("Somehow managed to set page index to " + m_pageIndex);
            m_pageIndex = totalPages - 1;
        }
        UpdateDisplay();
        m_pageLeftButton.Select();
    }

    public void SetCollectibleAmount(List<(CollectibleItem.ItemID id, int amount)> inventory)
    {
        m_sortedInventory = inventory;
        UpdateDisplay();
    }

    public void SetDisplay(bool shouldShow)
    {
        m_isCurrentlyShowing = shouldShow;
        if (shouldShow) SetCollectibleAmount(m_inventorySystem.Inventory);
        else
        {
            for (int i = 0; i < m_inventoryslots.Length; i++)
            {
                if (m_inventoryslots[i].IsCurrentlySelected())
                {
                    m_lastSelectedIndex = i;
                    break;
                }
            }
        }

        foreach (var slot in m_inventoryslots)
            slot.SetInteractibility(shouldShow);

        if (shouldShow) m_inventoryslots[m_lastSelectedIndex].SetSelected();

        m_rectTransform.anchoredPosition = shouldShow ? m_anchoredPos : m_outOfScreenPos;
    }

    public void UpdateDisplay(List<(CollectibleItem.ItemID, int)> inventory)
    {
        if (!m_isCurrentlyShowing) return;

        SetCollectibleAmount(inventory);
    }

    // Start is called before the first frame update
    void Start()
    {
        m_inventoryslots = GetComponentsInChildren<UIInventorySlotView>();
        m_rectTransform = GetComponent<RectTransform>();
        m_anchoredPos = m_rectTransform.anchoredPosition;

        m_rectTransform.anchoredPosition += 500.0f * Vector2.down;
        m_inventorySystem = InventorySystem.Instance;

        m_itemDatabase = m_inventorySystem.ItemDatabase;
        m_inventorySystem.ChangeItemAmtFinishedCb = UpdateDisplay;

        m_pageLeftButton.onClick.AddListener(PageLeft);
        m_pageRightButton.onClick.AddListener(PageRight);

        SetDisplay(false);
    }
}
