using UnityEngine;

namespace Skills.Structs
{
    [System.Serializable]
    public struct PlayerSkill
    {
        public enum SkillType
        {
            Invalid = 0,
            Dash,
            JumpCount,
            JumpDist,
            Roll,
            Fish,
            Count,
        }

        [field: SerializeField]
        public SkillType Type { get; set; }

        [field: SerializeField]
        public int Level { get; set; }
    }
}
