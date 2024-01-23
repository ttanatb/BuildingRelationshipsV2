using System;
using Dialogue;
using Dialogue.SO;
using UnityEngine;
using Dialogue.Struct;
using NaughtyAttributes;
using Utilr;
using Utilr.Attributes;

public class ActorNpc : InteractiblePropject
{
    [IncludeAllAssetsWithType]
    [SerializeField] private SetActiveNodeEvent[] m_setActiveNodeEvents = null;
    [SerializeField] private ActorAnimationEvent m_actorAnimEvent = null;
    [SerializeField] private MoveActorEvent m_moveActorEvent = null;
    
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

    private ActorPositionManager m_posManager = null;

    public StartDialogueData DialogueData =>
        new()
        {
            NodeName = m_dialogueNodeName,
        };

    protected override void Start()
    {
        base.Start();
        m_posManager = ActorPositionManager.Instance;
        ActorManager.Instance.Add(m_actorId, this);

        m_animator = GetComponent<Animator>();

        foreach (var evt in m_setActiveNodeEvents)
        {
            evt.Event.AddListener(OnActiveNodeSet);
        }
        
        m_actorAnimEvent.Event.AddListener(OnActorAnim);
        m_moveActorEvent.Event.AddListener(OnActorMove);
    }
 
    private void OnDestroy()
    {
        foreach (var evt in m_setActiveNodeEvents)
        {
            evt.Event.RemoveListener(OnActiveNodeSet);
        }
        
        m_actorAnimEvent.Event.RemoveListener(OnActorAnim);
        m_moveActorEvent.Event.RemoveListener(OnActorMove);
    }

    private void OnActiveNodeSet(SetActiveNodeData data)
    {
        if (data.ActorId != m_actorId)
            return;

        m_dialogueNodeName = data.NodeName;
    }

    private void OnActorAnim(ActorAnimationData data)
    {
        if (data.ActorId != m_actorId)
            return;

        switch (data.TriggerName)
        {
            case ActorAnimationData.AnimTrigger.StartTalk:
                m_animator.SetTrigger(m_animTriggerTalkStart);
                break;
            case ActorAnimationData.AnimTrigger.StopTalk:
                m_animator.SetTrigger(m_animTriggerTalkEnd);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    private void OnActorMove(MoveActorData data)
    {
        var anchor = m_posManager.Get(data.PositionId);
        var anchorTransform = anchor.transform;

        var thisTransform = transform;
        thisTransform.position = anchorTransform.position;
        thisTransform.rotation = anchorTransform.rotation;
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
}
