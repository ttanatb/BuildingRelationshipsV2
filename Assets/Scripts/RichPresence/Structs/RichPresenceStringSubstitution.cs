using UnityEngine;

namespace RichPresence.Structs
{
    [System.Serializable]
    public struct RichPresenceStringSubstitution
    {
        [field: SerializeField]
        public string Key { get; set; }
        
        [field: SerializeField]
        public string Value { get; set; }
    }
}
