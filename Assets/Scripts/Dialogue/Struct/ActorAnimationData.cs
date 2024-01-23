using UnityEngine;

namespace Dialogue.Struct
{
    [System.Serializable]
    public struct ActorAnimationData
    {
        [field: SerializeField]
        public string ActorId { get; set; }
        
        [field: SerializeField]
        public AnimTrigger TriggerName { get; set; }

        public enum AnimTrigger
        {
            StartTalk,
            StopTalk,
        }
    }
}
