using UnityEngine;

public class InteractiblePropject : MonoBehaviour
{

    [SerializeField]
    protected Transform m_dialogueBoxAnchor = null;

    [SerializeField]
    protected string m_interactText = "interact";

    protected UIManager m_uimanager = null;
    // Start is called before the first frame update
    protected virtual void Start()
    {
        m_uimanager = UIManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public virtual void SetInteractable(bool isInteractable)
    {
        if (isInteractable)
        {
            m_uimanager.SetCurrInteractAnchor(m_dialogueBoxAnchor, m_interactText);
        }
        else m_uimanager.SetCurrInteractAnchor(null);
    }
}
