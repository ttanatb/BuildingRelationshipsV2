using UnityEngine;

namespace Sound.Struct
{
    [System.Serializable]
    public struct OneShotAudioData
    {
        [field: SerializeField]
        public AudioClip Clip { get; set; }
        
        [field: SerializeField]
        public float Volume { get; set; }
        
        [field: SerializeField]
        public float Pitch { get; set; }
    }
}
