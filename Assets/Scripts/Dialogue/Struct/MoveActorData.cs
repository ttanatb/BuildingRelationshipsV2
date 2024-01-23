using UnityEngine;

namespace Dialogue.Struct
{
    [System.Serializable]
    public struct MoveActorData
    {
        [field: SerializeField]
        public string ActorId { get; set; }
        
        [field: SerializeField]
        public string PositionId { get; set; }
    }
}
