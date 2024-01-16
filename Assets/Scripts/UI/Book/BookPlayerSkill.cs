using System;
using TMPro;
using UnityEngine;
using Utilr.SoGameEvents;

namespace UI
{
    public class BookPlayerSkill : BookContent
    {
        [SerializeField] private PlayerSkill.Type m_playerSkill = PlayerSkill.Type.kInvalid;
        [SerializeField] private TextMeshPro m_levelText = null;

        private void Start()
        {
            var eventManager = EventManager.Instance;
            eventManager.AddSkillUnlockedListener(UpdateSkillLevel);
        }
        
        private void UpdateSkillLevel(PlayerSkill skill)
        {
            if (skill.type != m_playerSkill) return;

            m_levelText.text = $"Lv {skill.level}";
        }

        public override void SetVisible(bool isVisible)
        {
            // Nothing done here: the parent page class handles setting text visibility already    
        }
    }
}
