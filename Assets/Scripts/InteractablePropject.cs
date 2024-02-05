using UI.Events;
using UI.Structs;
using UnityEngine;
using UnityEngine.UI;
using Util.Localization.Structs;

public class InteractablePropject : MonoBehaviour
{
    [SerializeField]
    protected Transform m_dialogueBoxAnchor = null;

    [SerializeField]
    protected LocalizedText m_interactText = new LocalizedText()
    {
        Key = "verb_interact"
    };

    protected UIManager m_uiManager = null;
    protected bool m_interactable = false;
    
    [SerializeField] private PositionUiOnWorldPosEvent m_positionEvent = null;
    [SerializeField] private ShowInteractableMarkerEvent m_showEvent = null;
    [SerializeField] private HideInteractableMarkerEvent m_hideEvent = null;

    
    // Start is called before the first frame update
    protected virtual void Start()
    {
        m_uiManager = UIManager.Instance;
    }

    public virtual void SetInteractable(bool isInteractable)
    {
        m_interactable = isInteractable;
        
        if (isInteractable)
        {
            m_showEvent.Invoke(new ShowInteractableMarkerData()
            {
                Text = m_interactText
            });
        }
        else m_hideEvent.Invoke();
    }

    protected virtual void Update()
    {
        if (!m_interactable) return;
        
        m_positionEvent.Invoke(new PositionUiOnWorldPosData()
        {
            Position = m_dialogueBoxAnchor.position
        });
    }
}
