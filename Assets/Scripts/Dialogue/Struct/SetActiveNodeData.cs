using UnityEngine;

namespace Dialogue.Struct
{
    [System.Serializable]
    public struct SetActiveNodeData
    {
        [field: SerializeField]
        public string ActorId { get; set; }
        
        [field: SerializeField]
        public string NodeName { get; set; }
    }
}
