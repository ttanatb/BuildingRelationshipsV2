using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class UIInventorySlotView : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    const int MAX_QUANTITY = 9;

    [SerializeField]
    private Image m_image = null;

    [SerializeField]
    private TMP_Text m_nameText = null;

    [SerializeField]
    private TMP_Text m_descText = null;

    [SerializeField]
    private TMP_Text m_quantityText = null;

    [SerializeField]
    private GameObject m_bubblePanel = null;

    [SerializeField]
    private Button m_button = null;

    private Vector2 m_anchoredPos = Vector2.zero;
    [SerializeField]
    private Vector2 m_outOfScreenPos = Vector2.down * 1000;

    private RectTransform m_bubblePanelRect = null;

    [SerializeField]
    private RectTransform m_namePanelRect = null;

    private bool m_isEmpty = false;

    private bool m_isCurrentlySelected = false;

    // public CollectibleItem Collectible
    // {
    //     set
    //     {
    //         m_quantityText.text = "";
    //
    //         m_image.sprite = value.Sprite;
    //         m_nameText.text = value.DisplayName;
    //         m_descText.text = value.Description;
    //         LayoutRebuilder.ForceRebuildLayoutImmediate(m_bubblePanelRect);
    //         LayoutRebuilder.ForceRebuildLayoutImmediate(m_namePanelRect);
    //     }
    // }

    public int Amount
    {
        set
        {
            m_isEmpty = value == 0;
            if (value < 1)
            {
                m_image.color = new Color(0, 0, 0, 0);
                m_quantityText.text = string.Format("");
                return;
            }

            m_image.color = new Color(1, 1, 1, 1);

            if (value > MAX_QUANTITY)
            {
                m_quantityText.text = "9+";
            }
            else
            {
                m_quantityText.text = string.Format("{0}x", value.ToString());
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        m_button = GetComponent<Button>();

        m_bubblePanelRect = m_bubblePanel.GetComponent<RectTransform>();
        m_anchoredPos = m_bubblePanelRect.anchoredPosition;
        SetBubbleVisibility(false);
    }

    public void SetBubbleVisibility(bool isVisibile)
    {
        bool shouldShow = isVisibile;
        shouldShow = m_isEmpty ? false : isVisibile;
        m_bubblePanelRect.anchoredPosition = shouldShow ? m_anchoredPos : m_outOfScreenPos;
    }

    public void SetInteractibility(bool isVisible)
    {
        m_button.interactable = isVisible;
    }

    public void SetSelected()
    {
        m_button.Select();
    }

    public bool IsCurrentlySelected()
    {
        return m_isCurrentlySelected;
    }

    public void OnSelect(BaseEventData eventData)
    {
        SetBubbleVisibility(true);
        m_isCurrentlySelected = true;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        SetBubbleVisibility(false);
        m_isCurrentlySelected = false;
    }
}
