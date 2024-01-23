using System.Linq;
using Inventory.SerializedDict;
using Inventory.SO;
using Inventory.Structs;
using NaughtyAttributes;
using TMPro;
using UI.Structs;
using UnityEngine;
using UnityEngine.Serialization;

public class BookFishPanel : MonoBehaviour
{
    [Expandable] [SerializeField] private UiFishConfig m_config = null;

    [SerializeField] private ItemData.ItemID m_id = ItemData.ItemID.Invalid;
    public ItemData.ItemID Id {
        get => m_id;
        set
        {
            m_id = value;
            InitializeFromId();
        } 
    }
    
    [Expandable] [SerializeField] private ItemDatabase m_itemDb = null;
    private ItemData m_item = new ItemData();

    [SerializeField] private TextMeshPro m_nameText = null;
    [SerializeField] private TextMeshPro m_descText = null;
    [SerializeField] private TextMeshPro m_amountText = null;
    [SerializeField] private SpriteRenderer m_fishSprite = null;
    [SerializeField] private SpriteRenderer[] m_mediaBoxes = null;
    [SerializeField] private SpriteRenderer m_mainBox = null;
    [SerializeField] private SpriteRenderer m_bgBox = null;

    [SerializeField] private InventoryUpdatedEvent m_updatedInventoryEvent  = null;

    // Start is called before the first frame update
    private void Start()
    {
        m_updatedInventoryEvent.Event.AddListener(OnUpdateUi);
        InitializeFromId();
    }

    private void InitializeFromId()
    {
        m_item = m_itemDb.Dict.First(pair => pair.Value.Id == Id).Value;

        HideFish();
    }

    private void HideFish()
    {
        m_nameText.text = m_config.HiddenText;
        m_descText.text = m_config.HiddenText;

        m_fishSprite.sprite = m_item.Sprite;
        m_fishSprite.color = m_config.HiddenFishSpriteColor;
        foreach (var box in m_mediaBoxes)
            box.color = m_config.HiddenMediaBoxColor;

        m_mainBox.color = m_config.HiddenMainBoxColor;
        m_bgBox.color = m_config.HiddenBgBoxColor;
        m_amountText.text = "x0";
    }

    private void OnUpdateUi(ItemIdToItemCountDict countDict)
    {
        if (!countDict.TryGetValue(Id, out var item) || item.Count == 0)
        {
            // Item hasn't been obtained yet, keep it hidden.
            HideFish();
            return;
        }
        
        m_amountText.text = $"x{item.Count}";

        m_fishSprite.color = m_config.AvailableColor;
        foreach (var box in m_mediaBoxes)
            box.color = m_config.AvailableColor;
        
        m_mainBox.color = m_config.AvailableColor;
        m_bgBox.color = m_config.AvailableColor;
        
        m_nameText.text = m_item.DisplayName;
        m_descText.text = m_item.Description;
    }
}
