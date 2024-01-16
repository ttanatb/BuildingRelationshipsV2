using UnityEngine;

namespace Sound
{
    [System.Serializable]
    public struct NotesAtPointInSong
    {
        [field: SerializeField]
        public float TimestampSec { get; set; }
        
        [field: SerializeField]
        public AudioClip[] Clips { get; set; }
    }
}
