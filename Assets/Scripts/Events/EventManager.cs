using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Yarn.Unity;
using UnityEngine.Events;

public class EventManager : Singleton<EventManager>
{
    ItemEvent m_itemGainedEvent = new ItemEvent();
    UnityEvent m_dialogueCompletedEvent = new UnityEvent();
    FishReelStartEvent m_fishReelStartEvent = new FishReelStartEvent();
    FishReelEndedEvent m_fishReelEndedEvent = new FishReelEndedEvent();
    PlayerSkillUnlockedEvent m_skillUnlockedEvent = new PlayerSkillUnlockedEvent();
    FlagEvent m_flagEvent = new FlagEvent();

    bool m_fishReelStarted = false;

    [SerializeField]
    CollectibleItem.ItemID m_testItem = CollectibleItem.ItemID.Invalid;

    [SerializeField]
    int m_testItemAmt = 1;

    [SerializeField]
    PlayerSkill.Type m_testPlayerSkill = PlayerSkill.Type.kInvalid;

    [SerializeField]
    int m_testSkillLevel = 1;

    void Start()
    {
        Invoke(nameof(TestUnlockAllSkills), 5.0f);
    }

    public void FireTestItemEvent()
    {
        TriggerItemEvent(m_testItem, m_testItemAmt);
    }

    public void FireTestSkillUnlockedEvent()
    {
        TriggerSkillUnlockedEvent(new PlayerSkill() { type = m_testPlayerSkill, level = m_testSkillLevel });
    }
    public void TestUnlockAllSkills()
    {
        TriggerSkillUnlockedEvent(new PlayerSkill() { type = PlayerSkill.Type.Dash, level = 5 });
        TriggerSkillUnlockedEvent(new PlayerSkill() { type = PlayerSkill.Type.JumpCount, level = 5 });
        TriggerSkillUnlockedEvent(new PlayerSkill() { type = PlayerSkill.Type.JumpDist, level = 1 });
    }

    public void AddOneOfEachItem()
    {
        for (int i = 1; i < (int)CollectibleItem.ItemID.kCount - 1; i++)
        {
            TriggerItemEvent((CollectibleItem.ItemID)i, 1);
        }
    }

    public void AddItemListener(UnityAction<CollectibleItem.ItemID, int> cb)
    {
        m_itemGainedEvent.AddListener(cb);
    }

    [YarnCommand("addItem")]
    public void TriggerItemEvent(string itemName, string amount)
    {
        try
        {
            Enum.TryParse(itemName, out CollectibleItem.ItemID result);
            int.TryParse(amount, out int quantity);
            TriggerItemEvent(result, quantity);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    public void TriggerItemEvent(CollectibleItem.ItemID id, int amt)
    {
        if (id == CollectibleItem.ItemID.Invalid || id == CollectibleItem.ItemID.kCount)
        {
            Debug.LogError("Triggering Item Event with invalid id: " + id);
            return;
        }
        m_itemGainedEvent.Invoke(id, amt);
    }

    public void AddSkillUnlockedListener(UnityAction<PlayerSkill> cb)
    {
        m_skillUnlockedEvent.AddListener(cb);
    }

    [YarnCommand("unlockSkill")]
    public void TriggerSkillUnlockedEvent(string skillNameStr, string lvlStr)
    {
        try
        {
            Enum.TryParse(skillNameStr, out PlayerSkill.Type type);
            int.TryParse(lvlStr, out int lvl);
            TriggerSkillUnlockedEvent(
                new PlayerSkill { type = type, level = lvl });
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }



    public void TriggerSkillUnlockedEvent(PlayerSkill skill)
    {
        if (skill.type == PlayerSkill.Type.kInvalid || skill.type == PlayerSkill.Type.kCount)
        {
            Debug.LogError("Triggering Skill Unlocked Event with invalid type: " + skill.type);
            return;
        }
        m_skillUnlockedEvent.Invoke(skill);
    }

    public void TriggerDialogueCompletedEvent()
    {
        m_dialogueCompletedEvent.Invoke();
    }

    public void AddDialogueCompletedListener(UnityAction cb)
    {
        m_dialogueCompletedEvent.AddListener(cb);
    }

    public bool TriggerFishReelStartEvent(FishStats stats, Fish fish)
    {
        if (m_fishReelStarted)
            return false;

        m_fishReelStartEvent.Invoke(stats, fish);
        m_fishReelStarted = true;
        return true;
    }

    public void AddFishReelStartListener(UnityAction<FishStats, Fish> cb)
    {
        m_fishReelStartEvent.AddListener(cb);
    }

    public void TriggerFishReelEndedEvent(bool succeeded, CollectibleItem.ItemID id, Fish fish)
    {
        m_fishReelStarted = false;
        m_fishReelEndedEvent.Invoke(succeeded, id, fish);
    }

    public void AddFishReelEndedListener(UnityAction<bool, CollectibleItem.ItemID, Fish> cb)
    {
        m_fishReelEndedEvent.AddListener(cb);
    }

    [YarnCommand("setFlag")]
    public void SetEventFlag(string name)
    {
        try
        {
            TriggerEventFlag(name);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    public void TriggerEventFlag(string name)
    {
        m_flagEvent.Invoke(name);
    }

    public void AddEventFlagListener(UnityAction<string> cb)
    {
        m_flagEvent.AddListener(cb);
    }
}
