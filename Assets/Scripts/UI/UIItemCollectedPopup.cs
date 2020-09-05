using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIItemCollectedPopup : MonoBehaviour
{
    const string ANIM_TRIGGER_NAME = "Fade";


    [SerializeField]
    Image m_itemImage = null;

    [SerializeField]
    TextMeshProUGUI m_itemCollectedText = null;

    private Dictionary<CollectibleItem.ItemID, CollectibleItem> m_itemDatabase = null;
    private Animator m_animator = null;

    // Start is called before the first frame update
    void Start()
    {
        EventManager.Instance.AddItemListener(DisplayCollectedItem);
        m_itemDatabase = InventorySystem.Instance.ItemDatabase;
        m_animator = GetComponent<Animator>();
    }

    void DisplayCollectedItem(CollectibleItem.ItemID id, int amt)
    {
        if (!m_itemDatabase.ContainsKey(id))
        {
            Debug.LogError("Attempting to display item not in database.");
        }

        var item = m_itemDatabase[id];
        m_itemImage.sprite = item.sprite;
        if (amt == 0)
        {
            m_itemCollectedText.text =
                string.Format("Removed all {0}.", item.displayNamePlural);
        }

        if (amt < 0)
        {
            m_itemCollectedText.text =
                string.Format("Removed {0}x {1}", Mathf.Abs(amt),
                amt == -1 ? item.displayName : item.displayNamePlural);
        }

        if (amt > 0)
        {
            m_itemCollectedText.text =
                string.Format("Obtained {0}x {1}", amt,
                amt == 1 ? item.displayName : item.displayNamePlural);
        }

        m_animator.SetTrigger(ANIM_TRIGGER_NAME);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
