using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerTalk : MonoBehaviour
{
    [SerializeField]
    LayerMask m_npcLayerMask = 0;

    [SerializeField]
    TalkableNPC m_currTalkableNPC = null;

    Yarn.Unity.DialogueRunner m_dialogueRunner = null;

    PlayerInput m_playerInput = null;
    PlayerControls m_playerControls = null;

    PlayerMovement m_playerMovement = null;

    // Start is called before the first frame update
    void Start()
    {
        m_dialogueRunner = DialogueManager.Instance.DialogueRunner;
        m_playerControls = GetComponent<PlayerController>().PlayerControls;
        m_playerInput = GetComponent<PlayerInput>();
        m_playerMovement = GetComponent<PlayerMovement>();

        m_playerControls.Player.Interact.performed += TriggerDialogue;
    }

    private void OnDestroy()
    {
        m_playerControls.Player.Interact.performed -= TriggerDialogue;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void TriggerDialogue(InputAction.CallbackContext context)
    {
        if (m_currTalkableNPC == null)
        {
            return;
        }

        m_dialogueRunner.StartDialogue(m_currTalkableNPC.DialogueNodeName);
        m_playerMovement.StopMovement();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (m_npcLayerMask == (m_npcLayerMask | (1 << other.gameObject.layer)))
        {
            TalkableNPC npc = other.GetComponent<TalkableNPC>();
            if (npc == null)
            {
                Debug.LogError("Entered trigger of chracter who is not NPC.");
                return;
            }

            if (m_currTalkableNPC != null)
            {
                Debug.LogWarning("Overriding current NPC to talk to.");
                m_currTalkableNPC.SetInteractable(false);
            }

            string dialogueNodeName = npc.DialogueNodeName;

            // Check NodeExists
            if (!m_dialogueRunner.NodeExists(dialogueNodeName))
            {
                Debug.LogError(string.Format(
                    "Dialogue node with name ({0}) does not exist.", dialogueNodeName));
                m_currTalkableNPC = null;
                return;
            }

            m_currTalkableNPC = npc;
            m_currTalkableNPC.SetInteractable(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (m_npcLayerMask == (m_npcLayerMask | (1 << other.gameObject.layer)))
        {
            TalkableNPC npc = other.GetComponent<TalkableNPC>();
            if (npc == null)
            {
                Debug.LogError("Exited trigger of chracter who is not NPC.");
                return;
            }

            if (m_currTalkableNPC != npc)
            {
                Debug.LogWarning("Exiting NPC range of an NPC you weren't in range of.");
                npc.SetInteractable(false);
            }

            m_currTalkableNPC.SetInteractable(false);
            m_currTalkableNPC = null;
        }
    }
}
