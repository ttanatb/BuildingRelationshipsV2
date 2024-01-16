using UnityEngine;
using System.Text;
using System.Collections;


[System.Serializable]
public struct PlayerSkill
{
    public enum Type
    {
        kInvalid = 0,
        Dash,
        JumpCount,
        JumpDist,
        Roll,
        Fish,
        kCount,
    }

    public Type type;
    public int level;
}

