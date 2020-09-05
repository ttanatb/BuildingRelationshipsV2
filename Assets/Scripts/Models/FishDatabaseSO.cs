using UnityEngine;
using System.Text;
using System.Collections;


[System.Serializable]
public struct FishStats
{
    public CollectibleItem.ItemID id;
    public float DecayRate;
    public float CompletionRate;
    public float JumpIntervalSec;
    public float FishLerpRate;
    public Vector2 MinJumpDistance;
    public Vector2 MaxJumpDistance;

    public Vector2 MinBounds;
    public Vector2 MaxBounds;

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendFormat("ID ({0})\n", id);
        sb.AppendFormat("DecayRate ({0})\n", DecayRate);
        sb.AppendFormat("CompletionRate ({0})\n", CompletionRate);
        sb.AppendFormat("JumpIntervalSec ({0})\n", JumpIntervalSec);
        sb.AppendFormat("FishLerpRate ({0})\n", FishLerpRate);
        sb.AppendFormat("MinJumpDistRatio({0})\n", MinJumpDistance);
        sb.AppendFormat("MaxJumpDistance({0})\n", MaxJumpDistance);
        return sb.ToString();
    }
}

public class FishDatabaseSO : ScriptableObject
{
    public FishStats[] fishStats;
}
