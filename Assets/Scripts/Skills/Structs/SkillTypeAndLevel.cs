using UnityEngine;

namespace Skills.Structs
{
    [System.Serializable]
    public struct SkillTypeAndLevel
    {
        [field: SerializeField] 
        public PlayerSkill.SkillType SkillType { get; set; }
        
        [field: SerializeField]
        public int Level { get; set; }
    }
}
