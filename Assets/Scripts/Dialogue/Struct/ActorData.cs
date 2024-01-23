using System.Text;
using Dialogue.SO;
using UnityEngine;

namespace Dialogue.Struct
{
    /// <summary>
    /// ????
    /// </summary>
    [System.Serializable]
    public struct ActorData
    {
        [field: SerializeField]
        public string ID { get; set; }
        
        [field: SerializeField]
        public Transform Anchor { get; set; }
        
        // [field: SerializeField]
        // public ActorAnimationEvent AnimationEvent { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append($"{ID}: Anchor ({Anchor}");
            // AnimEvent ({AnimationEvent})");
            return sb.ToString();
        }
    }
}
