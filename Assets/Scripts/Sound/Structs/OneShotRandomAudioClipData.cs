using NaughtyAttributes;
using UnityEngine;

namespace Sound.Struct
{
    [System.Serializable]
    public struct OneShotRandomAudioClipData
    {
        [field: SerializeField]
        public AudioClip[] Clip { get; set; }
        
        [field: MinMaxSlider(0f, 1f)]
        [field: SerializeField]
        public Vector2 Volume { get; set; }
        
        [field: MinMaxSlider(0f, 2f)]
        [field: SerializeField]
        public Vector2 Pitch { get; set; }
        
        [field: SerializeField]
        public Vector3 Position { get; set; }
    }
}
