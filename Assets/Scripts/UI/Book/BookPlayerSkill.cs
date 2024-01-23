using Skills.SO;
using Skills.Structs;
using TMPro;
using UnityEngine;

namespace UI
{
    public class BookPlayerSkill : BookContent
    {
        [SerializeField] private SetSkillLevelEvent m_setSkillLevelEvent = null;
        [SerializeField] private PlayerSkill.SkillType m_playerSkill = PlayerSkill.SkillType.Invalid;
        [SerializeField] private TextMeshPro m_levelText = null;

        private void Start()
        {
            m_setSkillLevelEvent.Event.AddListener(UpdateSkillLevel);
        }

        private void OnDestroy()
        {
            m_setSkillLevelEvent.Event.RemoveListener(UpdateSkillLevel);
        }

        private void UpdateSkillLevel(SkillTypeAndLevel data)
        {
            if (data.SkillType != m_playerSkill) return;
            m_levelText.text = $"Lv {data.Level}";
        }

        public override void SetVisible(bool isVisible)
        {
            // Nothing done here: the parent page class handles setting text visibility already    
        }
    }
}
