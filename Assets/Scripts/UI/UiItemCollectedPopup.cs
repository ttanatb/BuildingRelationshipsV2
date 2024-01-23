using System;
using Inventory.SO;
using Inventory.Structs;
using NaughtyAttributes;
using Sound.Struct;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;
using Utilr.Utility;

public class UiItemCollectedPopup : MonoBehaviour
{
    [SerializeField] private Image m_itemImage = null;
    [SerializeField] private TextMeshProUGUI m_itemCollectedText = null;
    [SerializeField] private ItemDatabase m_itemDatabase = null;
    
    [SerializeField] private Animator m_animator = null;
    
    [AnimatorParam("m_animator")]
    [SerializeField] private int m_showAnimParam = 0;
    
    [AnimatorParam("m_animator")]
    [SerializeField] private int m_hideAnimParam = 0;
    
    [AnimatorParam("m_animator")]
    [SerializeField] private int m_moveUpAnimParam = 0;

    [SerializeField] private PlayOneShotRandomAudioClipEvent m_obtainAudio = null;

    [SerializeField] private AnimationClip m_hideClip = null;

    private RectTransform m_rectTransform = null;

    private void Awake()
    {
        TryGetComponent(out m_rectTransform);
    }

    public void Display(ItemData.ItemID itemId, float displayDuration, UnityAction onDisplayCompletedCb)
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(m_rectTransform);

        if (!m_itemDatabase.Dict.TryGetValue(itemId, out var item))
        {
            Debug.LogError("Attempting to display item not in database.");
        }

        m_itemImage.sprite = item.Sprite;
        m_itemCollectedText.text = $"Got {item.DisplayName}";

        m_animator.SetTrigger(m_showAnimParam);
        m_obtainAudio.Invoke();
        StartCoroutine(Helper.ExecuteAfter(() => {
            m_animator.SetTrigger(m_hideAnimParam);
            StartCoroutine(onDisplayCompletedCb.ExecuteAfter(m_hideClip.length));
        }, displayDuration));
    }

    public void EarlyExit()
    {
        StopAllCoroutines();
        m_animator.SetTrigger(m_hideAnimParam);
    }

    public void MoveUp()
    {
        m_animator.SetTrigger(m_moveUpAnimParam);
    }
}
