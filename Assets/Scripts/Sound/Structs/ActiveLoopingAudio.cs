using System.Collections;
using UnityEngine;

namespace Sound.Structs
{
    public struct ActiveLoopingAudio
    {
        [field: SerializeField] 
        public AudioSource AudioSource { get; set; }
        
        [field: SerializeField]
        public int PoolIndex { get; set; }
        
        [field: SerializeField]
        public IEnumerator FadeInCoroutine { get; set; }
        
        [field: SerializeField]
        public Transform Target { get; set; }
    }
}
