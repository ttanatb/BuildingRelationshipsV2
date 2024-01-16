using System.Linq;
using NaughtyAttributes;
using TMPro;
using UI.Structs;
using UnityEngine;

namespace UI
{
    public class BookPlayerItem : BookContent
    {
        [Expandable] [SerializeField] private UiFishConfig m_config = null;

        [SerializeField] private CollectibleItem.ItemID m_id = CollectibleItem.ItemID.Invalid;
        private CollectibleItem m_item;
        
        [SerializeField] private SoUpdateItemEvent m_updateItemEvent = null;

        [Expandable] [SerializeField] private CollectibleSO m_itemDb = null;

        [SerializeField] private TextMeshPro m_nameText = null;
        [SerializeField] private TextMeshPro m_amountText = null;
        [SerializeField] private SpriteRenderer m_fishSprite = null;
        [SerializeField] private SpriteRenderer[] m_mainBoxes = null;
        [SerializeField] private SpriteRenderer[] m_bgBoxes = null;
        [SerializeField] private bool m_hideAmt = false;
        
        public CollectibleItem.ItemID Id {
            get => m_id;
            set
            {
                m_id = value;
                InitializeFromId();
            } 
        }
        
        private void Start()
        {
            m_updateItemEvent.Event.AddListener(OnUpdateUi);
            InitializeFromId();
        
        }

        private void InitializeFromId()
        {
            m_item = m_itemDb.items.First(item => item.id == Id);

            m_fishSprite.sprite = m_item.sprite;
            m_fishSprite.color = m_config.HiddenFishSpriteColor;
            foreach (var box in m_mainBoxes)
                box.color = m_config.HiddenMainBoxColor;

            foreach (var box in m_bgBoxes)
                box.color = m_config.HiddenBgBoxColor;

            m_amountText.text = "";
            m_nameText.text = "";
        }

        private void OnUpdateUi(UpdateItemStatus item)
        {
            if (item.id != Id) return;
        
            if (!m_hideAmt)
                m_amountText.text = $"x{item.totalCount}";

            if (item.totalCount <= 0)
                return;
        
            m_fishSprite.color = m_config.AvailableColor;
            foreach (var box in m_mainBoxes)
                box.color = m_config.AvailableColor;

            foreach (var box in m_bgBoxes)
                box.color = m_config.AvailableColor;
        
            m_nameText.text = m_item.displayName;
        }
        
        public override void SetVisible(bool isVisible)
        {
            // Nothing done here: the parent page class handles setting text visibility already    
        }
    }
}
