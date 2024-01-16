using System;
using Dialogue.SO;
using UnityEngine;
using Dialogue.Struct;
using NaughtyAttributes;
using UnityEditor.Experimental.GraphView;
using Utilr;
using Utilr.Attributes;

public class TalkableNPC : InteractiblePropject
{
    [IncludeAllAssetsWithType]
    [SerializeField] private SetActiveNodeEvent[] m_setActiveNodeEvents = null;
    
    [SerializeField] private string m_actorId = "";
    [SerializeField] private string m_dialogueNodeName = "";
    [SerializeField] private LayerMask m_playerLayer = 1 << 10;
    
    [SerializeField]
    private Animator m_animator = null;

    [SerializeField] [AnimatorParam("m_animator")]
    private int m_animTriggerTalkStart = 1;

    [SerializeField] [AnimatorParam("m_animator")]
    private int m_animTriggerTalkEnd = 1;

    [SerializeField] [AnimatorParam("m_animator")]
    private int m_animTriggerInRadius = 1;

    [SerializeField] [AnimatorParam("m_animator")]
    private int m_animTriggerExitRadius = 1;

    public Transform DialogueBoxAnchor => m_dialogueBoxAnchor;

    public StartDialogueData DialogueData =>
        new()
        {
            NodeName = m_dialogueNodeName,
        };

    protected override void Start()
    {
        base.Start();
        m_animator = GetComponent<Animator>();

        foreach (var evt in m_setActiveNodeEvents)
        {
            evt.Event.AddListener(OnActiveNodeSet);
        }
    }

    private void OnDestroy()
    {
        foreach (var evt in m_setActiveNodeEvents)
        {
            evt.Event.RemoveListener(OnActiveNodeSet);
        }
    }

    private void OnActiveNodeSet(SetActiveNodeData data)
    {
        if (data.ActorId != m_actorId)
            return;

        m_dialogueNodeName = data.NodeName;
    }

    private void OnTriggerEnter(Collider other)
    {
        var otherGameObj = other.gameObject;
        if (!m_playerLayer.ContainsLayer(otherGameObj.layer))
            return;
        
        m_uimanager.SetCurrInteractAnchor(m_dialogueBoxAnchor, m_interactText);
        m_animator.SetTrigger(m_animTriggerInRadius);
    }

    private void OnTriggerExit(Collider other)
    {
        var otherGameObj = other.gameObject;
        if (!m_playerLayer.ContainsLayer(otherGameObj.layer))
            return;
        
        m_animator.SetTrigger(m_animTriggerExitRadius);
        m_uimanager.SetCurrInteractAnchor(null);
    }

    public void SetTalkAnim(bool start)
    {
        // if (start)
        // {
        //     m_animator.SetTrigger(m_animTriggerTalkStart);
        // }
        // else
        // {
        //     m_animator.SetTrigger(m_animTriggerTalkEnd);
        // }
    }

    public void SetInteractable(bool isInteractable, bool triggerAnim = true)
    {
        // if (isInteractable)
        // {
        //     m_uimanager.SetCurrInteractAnchor(m_dialogueBoxAnchor, m_interactText);
        // }
        // else m_uimanager.SetCurrInteractAnchor(null);
        //
        // if (!triggerAnim)
        //     return;
        //
        // if (isInteractable)
        // {
        //     m_animator.SetTrigger(m_animTriggerInRadius);
        // }
        // else
        // {
        //     m_animator.SetTrigger(m_animTriggerExitRadius);
        // }
    }
}
