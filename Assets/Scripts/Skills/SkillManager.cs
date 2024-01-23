using Inventory.SerializedDict;
using Inventory.SO;
using Skills.SerializedDict;
using Skills.SO;
using Skills.Structs;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

namespace Skills
{
    public class SkillManager : Utilr.Singleton<SkillManager>
    {
        [SerializeField] private ItemIdToSkillTypeDict m_itemIdToSkillTypeDict = new ItemIdToSkillTypeDict();
        [SerializeField] private InventoryUpdatedEvent m_inventoryUpdatedEvent = null;
        [SerializeField] private SetSkillLevelEvent m_setSkillLevelEvent = null;
        [SerializeField] private InputActionReference m_debugGrantAllSkills = null;

        private void Start()
        {
            Assert.IsNotNull(m_setSkillLevelEvent);
            
            m_inventoryUpdatedEvent.Event.AddListener(OnInventoryUpdated);
            m_debugGrantAllSkills.action.performed += GrantAllSkills;
        }

        private void OnDestroy()
        {
            m_inventoryUpdatedEvent.Event.RemoveListener(OnInventoryUpdated);
            m_debugGrantAllSkills.action.performed -= GrantAllSkills;

        }
        
        private void GrantAllSkills(InputAction.CallbackContext _)
        {
            m_setSkillLevelEvent.Invoke(new SkillTypeAndLevel()
            {
                SkillType = PlayerSkill.SkillType.JumpCount,
                Level = 10,
            });
            
            m_setSkillLevelEvent.Invoke(new SkillTypeAndLevel()
            {
                SkillType = PlayerSkill.SkillType.JumpDist,
                Level = 10,
            });

            m_setSkillLevelEvent.Invoke(new SkillTypeAndLevel()
            {
                SkillType = PlayerSkill.SkillType.Dash,
                Level = 10,
            });
            
            m_setSkillLevelEvent.Invoke(new SkillTypeAndLevel()
            {
                SkillType = PlayerSkill.SkillType.Fish,
                Level = 10,
            });
        }
        
        private void OnInventoryUpdated(ItemIdToItemCountDict inventory)
        {
            foreach (var pair in inventory)
            {
                if (m_itemIdToSkillTypeDict.TryGetValue(pair.Key, out var skill))
                {
                    m_setSkillLevelEvent.Invoke(new SkillTypeAndLevel()
                    {
                        SkillType = skill,
                        Level = pair.Value.Count,
                    });
                }
            }
        }
    }
}
