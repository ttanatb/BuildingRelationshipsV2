using UnityEngine;
using UnityEngine.InputSystem;
using Dialogue.SO;
using Input.SO;

public class PlayerTalk : MonoBehaviour
{
    [SerializeField] private LayerMask m_npcLayerMask = 0;

    [SerializeField] private TalkableNPC m_currTalkableNPC = null;

    [SerializeField] private InputActionReference m_interactInput = null;

    [SerializeField] private StartDialogueEvent m_startDialogueEvent = null;
    [SerializeField] private StopDialogueEvent m_stopDialogueEvent = null;

    private EventManager m_eventManager = null;

    // Start is called before the first frame update
    void Start()
    {
        m_eventManager = EventManager.Instance;
        m_eventManager.AddDialogueCompletedListener(OnDialogueCompletion);

        m_interactInput.action.performed += TriggerDialogue;
        
        m_stopDialogueEvent.Event.AddListener(OnDialogueCompletion);
    }

    private void OnDestroy()
    {
        m_interactInput.action.performed -= TriggerDialogue;

        m_stopDialogueEvent.Event.RemoveListener(OnDialogueCompletion);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnDialogueCompletion()
    {
        if (m_currTalkableNPC == null)
        {
            return;
        }
        if (!m_currTalkableNPC.gameObject.activeSelf)
        {
            m_currTalkableNPC = null;
            return;
        }

        m_currTalkableNPC.SetTalkAnim(false);
        m_currTalkableNPC.SetInteractable(true, false);
    }

    public void TriggerDialogue(InputAction.CallbackContext context)
    {
        if (m_currTalkableNPC == null)
        {
            return;
        }
        
        m_startDialogueEvent.Invoke(m_currTalkableNPC.DialogueData);
        m_currTalkableNPC.SetInteractable(false, false);
        m_currTalkableNPC.SetTalkAnim(true);
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (m_npcLayerMask != (m_npcLayerMask | (1 << other.gameObject.layer)))
            return;
        
        other.TryGetComponent(out TalkableNPC npc);
        if (npc == null)
        {
            Debug.LogError($"Entered trigger of character who is not NPC: {other.gameObject}");
            return;
        }

        if (npc != m_currTalkableNPC && m_currTalkableNPC != null)
        {
            Debug.LogWarning($"Overriding current NPC ({m_currTalkableNPC.gameObject}) with ({npc.gameObject}).");
            m_currTalkableNPC.SetInteractable(false);
        }

        m_currTalkableNPC = npc;
        m_currTalkableNPC.SetInteractable(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (m_npcLayerMask != (m_npcLayerMask | (1 << other.gameObject.layer)))
            return;

        other.TryGetComponent(out TalkableNPC npc);
        if (npc == null)
        {
            Debug.LogError("Exited trigger of character who is not NPC.");
            return;
        }

        if (m_currTalkableNPC != npc && m_currTalkableNPC != null)
        {
            Debug.LogWarning($"Exiting NPC range of {other.gameObject},"
                + $" last cached was {m_currTalkableNPC.gameObject}.");
        }
        
        npc.SetInteractable(false);

        if (m_currTalkableNPC == null)
            return;

        m_currTalkableNPC.SetInteractable(false);
        m_currTalkableNPC = null;
    }
}
