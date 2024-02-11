using UnityEngine;
using Utilr.Structs;

namespace Sound.Structs
{
    [System.Serializable]
    public struct LoopingAudioData
    {
        [field: SerializeField]
        public AudioClip Clip { get; set; }

        [field: SerializeField]
        public float Volume { get; set; }

        [field: SerializeField]
        public LerpAnimData<float> FadeIn { get; set; }
        
        [field: SerializeField]
        public LerpAnimData<float> FadeOut { get; set; }
        
        [field: SerializeField]
        public Transform Target { get; set; }
    }
}
