using UnityEngine;
using UnityEngine.InputSystem;
using Dialogue.SO;
using Utilr;

public class PlayerTalk : MonoBehaviour
{
    [SerializeField] private LayerMask m_npcLayerMask = 0;
    [SerializeField] private ActorNpc m_currActorNpc = null;
    [SerializeField] private InputActionReference m_interactInput = null;
    [SerializeField] private StartDialogueEvent m_startDialogueEvent = null;
    [SerializeField] private StopDialogueEvent m_stopDialogueEvent = null;

    // Start is called before the first frame update
    private void Start()
    {
        m_interactInput.action.performed += TriggerDialogue;
        
        m_stopDialogueEvent.Event.AddListener(OnDialogueCompletion);
    }

    private void OnDestroy()
    {
        m_interactInput.action.performed -= TriggerDialogue;

        m_stopDialogueEvent.Event.RemoveListener(OnDialogueCompletion);
    }

    private void OnDialogueCompletion()
    {
        if (m_currActorNpc == null)
            return;

        if (m_currActorNpc.gameObject.activeSelf)
            return;
        
        m_currActorNpc = null;
    }

    private void TriggerDialogue(InputAction.CallbackContext context)
    {
        if (m_currActorNpc == null)
            return;
        
        m_startDialogueEvent.Invoke(m_currActorNpc.DialogueData);
    }

    private void OnTriggerEnter(Collider other)
    {
        var otherGameObj = other.gameObject;
        if (!m_npcLayerMask.ContainsLayer(otherGameObj.layer))
            return;
        
        other.TryGetComponent(out ActorNpc npc);
        if (npc == null)
        {
            Debug.LogError($"Entered trigger of character who is not NPC: {otherGameObj}");
            return;
        }

        if (npc != m_currActorNpc && m_currActorNpc != null)
        {
            Debug.LogWarning($"Overriding current NPC ({m_currActorNpc.gameObject}) with ({npc.gameObject}).");
        }

        m_currActorNpc = npc;
    }

    private void OnTriggerExit(Collider other)
    {
        var otherGameObj = other.gameObject;
        if (!m_npcLayerMask.ContainsLayer(otherGameObj.layer))
            return;

        other.TryGetComponent(out ActorNpc npc);
        if (npc == null)
        {
            Debug.LogError("Exited trigger of character who is not NPC.");
            return;
        }

        if (m_currActorNpc != npc && m_currActorNpc != null)
        {
            Debug.LogWarning($"Exiting NPC range of {otherGameObj},"
                + $" last cached was {m_currActorNpc.gameObject}.");
        }
        
        m_currActorNpc = null;
    }
}
