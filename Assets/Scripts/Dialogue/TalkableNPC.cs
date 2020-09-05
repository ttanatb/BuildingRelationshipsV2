using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

public class TalkableNPC : InteractiblePropject
{
    const string ANIM_TRIG_TALK_START = "talkStart";
    const string ANIM_TRIG_TALK_END = "talkEnd";
    const string ANIM_TRIG_ENTER_RADIUS = "enterRadius";
    const string ANIM_TRIG_EXIT_RADIUS = "exitRadius";

    [SerializeField]
    string m_dialogueNodeName = "";

    Animator m_animator = null;

    public Transform DialogueBoxAnchor { get { return m_dialogueBoxAnchor; } }

    public string DialogueNodeName { get { return m_dialogueNodeName; } }

    protected override void Start()
    {
        base.Start();
        m_animator = GetComponent<Animator>();
    }

    public void SetTalkAnim(bool start)
    {
        if (start)
        {
            m_animator.SetTrigger(ANIM_TRIG_TALK_START);
        }
        else
        {
            m_animator.SetTrigger(ANIM_TRIG_TALK_END);
        }
    }

    public void SetInteractable(bool isInteractable, bool triggerAnim = true)
    {
        if (isInteractable)
        {
            m_uimanager.SetCurrInteractAnchor(m_dialogueBoxAnchor, m_interactText);
        }
        else m_uimanager.SetCurrInteractAnchor(null);

        if (!triggerAnim)
            return;

        if (isInteractable)
        {
            m_animator.SetTrigger(ANIM_TRIG_ENTER_RADIUS);
        }
        else
        {
            m_animator.SetTrigger(ANIM_TRIG_EXIT_RADIUS);
        }
    }
}
