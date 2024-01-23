using System.Linq;
using Inventory.SerializedDict;
using Inventory.SO;
using Inventory.Structs;
using NaughtyAttributes;
using TMPro;
using UI.Structs;
using UnityEngine;
using UnityEngine.Serialization;

namespace UI
{
    public class BookPlayerItem : BookContent
    {
        [Expandable] [SerializeField] private UiFishConfig m_config = null;

        [SerializeField] private ItemData.ItemID m_id = ItemData.ItemID.Invalid;
        private ItemData m_item = new ItemData();
        
        [SerializeField] private InventoryUpdatedEvent m_updatedInventoryEvent  = null;

        [Expandable] [SerializeField] private ItemDatabase m_itemDb = null;

        [SerializeField] private TextMeshPro m_nameText = null;
        [SerializeField] private TextMeshPro m_amountText = null;
        [SerializeField] private SpriteRenderer m_fishSprite = null;
        [SerializeField] private SpriteRenderer[] m_mainBoxes = null;
        [SerializeField] private SpriteRenderer[] m_bgBoxes = null;
        [SerializeField] private bool m_hideAmt = false;
        
        public ItemData.ItemID Id {
            get => m_id;
            set
            {
                m_id = value;
                InitializeFromId();
            } 
        }
        
        private void Start()
        {
            m_updatedInventoryEvent.Event.AddListener(OnUpdateUi);
            InitializeFromId();
        }

        private void InitializeFromId()
        {
            m_item = m_itemDb.Dict.First(pair => pair.Value.Id == Id).Value;
            m_fishSprite.sprite = m_item.Sprite;
            HideItem();
        }

        private void HideItem()
        {
            m_fishSprite.color = m_config.HiddenFishSpriteColor;
            foreach (var box in m_mainBoxes)
                box.color = m_config.HiddenMainBoxColor;

            foreach (var box in m_bgBoxes)
                box.color = m_config.HiddenBgBoxColor;

            m_amountText.text = "";
            m_nameText.text = "";
        }

        private void OnUpdateUi(ItemIdToItemCountDict countDict)
        {
            if (!countDict.TryGetValue(Id, out var item) || item.Count == 0)
            {
                // Item hasn't been obtained yet, keep it hidden.
                HideItem();
                return;
            }
        
            if (!m_hideAmt)
                m_amountText.text = $"x{item.Count}";

            m_fishSprite.color = m_config.AvailableColor;
            foreach (var box in m_mainBoxes)
                box.color = m_config.AvailableColor;

            foreach (var box in m_bgBoxes)
                box.color = m_config.AvailableColor;
        
            m_nameText.text = m_item.DisplayName;
        }
        
        public override void SetVisible(bool isVisible)
        {
            // Nothing done here: the parent page class handles setting text visibility already    
        }
    }
}
