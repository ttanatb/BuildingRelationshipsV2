using System.Text;
using Inventory.Structs;
using UnityEngine;

namespace Fishing.Structs
{
    [System.Serializable]
    public struct FishData
    {
        [field: SerializeField]
        public ItemData.ItemID Id { get; set; }

        [field: SerializeField]
        public float DecayRate { get; set; }

        [field: SerializeField]
        public float CompletionRate { get; set; }

        [field: SerializeField]
        public float JumpIntervalSec { get; set; }

        [field: SerializeField]
        public float FishLerpRate { get; set; }

        [field: SerializeField]
        public Vector2 MinJumpDistance { get; set; }

        [field: SerializeField]
        public Vector2 MaxJumpDistance { get; set; }

        [field: SerializeField]
        public Vector2 MinBounds { get; set; }

        [field: SerializeField]
        public Vector2 MaxBounds { get; set; }


        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("ID ({0})\n", Id);
            sb.AppendFormat("DecayRate ({0})\n", DecayRate);
            sb.AppendFormat("CompletionRate ({0})\n", CompletionRate);
            sb.AppendFormat("JumpIntervalSec ({0})\n", JumpIntervalSec);
            sb.AppendFormat("FishLerpRate ({0})\n", FishLerpRate);
            sb.AppendFormat("MinJumpDistRatio({0})\n", MinJumpDistance);
            sb.AppendFormat("MaxJumpDistance({0})\n", MaxJumpDistance);
            return sb.ToString();
        }
    }
}
