using System.Linq;
using NaughtyAttributes;
using TMPro;
using UI.Structs;
using UnityEngine;
using UnityEngine.Serialization;

public class UiFishPanel : MonoBehaviour
{
    [Expandable] [SerializeField] private UiFishConfig m_config = null;

    [SerializeField] private CollectibleItem.ItemID m_id = CollectibleItem.ItemID.Invalid;
    public CollectibleItem.ItemID Id {
        get => m_id;
        set
        {
            m_id = value;
            InitializeFromId();
        } 
    }
    
    [Expandable] [SerializeField] private CollectibleSO m_itemDb = null;
    private CollectibleItem m_item;

    [SerializeField] private TextMeshPro m_nameText = null;
    [SerializeField] private TextMeshPro m_descText = null;
    [SerializeField] private TextMeshPro m_amountText = null;
    [SerializeField] private SpriteRenderer m_fishSprite = null;
    [SerializeField] private SpriteRenderer[] m_mediaBoxes = null;
    [SerializeField] private SpriteRenderer m_mainBox = null;
    [SerializeField] private SpriteRenderer m_bgBox = null;

    [SerializeField] private SoUpdateItemEvent m_updateItemEvent = null;
    
    // Start is called before the first frame update
    private void Start()
    {
        m_updateItemEvent.Event.AddListener(OnUpdateUi);
        InitializeFromId();
        
    }

    private void InitializeFromId()
    {
        m_item = m_itemDb.items.First(item => item.id == Id);

        m_nameText.text = m_config.HiddenText;
        m_descText.text = m_config.HiddenText;

        m_fishSprite.sprite = m_item.sprite;
        m_fishSprite.color = m_config.HiddenFishSpriteColor;
        foreach (var box in m_mediaBoxes)
            box.color = m_config.HiddenMediaBoxColor;

        m_mainBox.color = m_config.HiddenMainBoxColor;
        m_bgBox.color = m_config.HiddenBgBoxColor;
        m_amountText.text = "x0";
    }

    private void OnUpdateUi(UpdateItemStatus item)
    {
        if (item.id != Id) return;
        
        m_amountText.text = $"x{item.totalCount}";

        if (item.totalCount <= 0)
            return;
        
        m_fishSprite.color = m_config.AvailableColor;
        foreach (var box in m_mediaBoxes)
            box.color = m_config.AvailableColor;
        
        m_mainBox.color = m_config.AvailableColor;
        m_bgBox.color = m_config.AvailableColor;
        
        m_nameText.text = m_item.displayName;
        m_descText.text = m_item.description;
    }
}
