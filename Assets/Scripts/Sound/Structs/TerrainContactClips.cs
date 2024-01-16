using UnityEngine;

namespace Sound.Structs
{
    [System.Serializable]
    public struct TerrainContactClips
    {
        [field: SerializeField]
        public AudioClip[] StepClips { get; set; }
        
        [field: SerializeField]
        public AudioClip[] SlideClips { get; set; }
        
        [field: SerializeField]
        public AudioClip[] ThudClips { get; set; }

        private static int m_counter;

        public AudioClip GetNext(AudioClip[] clips)
        {
            return clips[m_counter++ % clips.Length];
        }
    }
}
