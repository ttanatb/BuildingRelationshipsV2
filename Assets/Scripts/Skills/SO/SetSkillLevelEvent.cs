using Skills.Structs;
using UnityEngine;
using Utilr.SoGameEvents;

namespace Skills.SO
{
    [CreateAssetMenu(fileName = "SetSkillLevelEvent", menuName = "br/Skills/SetSkillLevelEvent", order = 1)]
    public class SetSkillLevelEvent : SoCustomEvent<SkillTypeAndLevel>
    {
        
    }
}
