using Sound.Structs;
using UnityEngine;

namespace Sound.SerializedDict
{
    [System.Serializable]
    public class TextureToAudioClipsDict : SerializableDictionary<Texture, TerrainContactClips>
    {
        // private int m_counter = 0;
        //
        // public AudioClip GetNext(Texture texture)
        // { 
        //     var clips = this[texture];
        //     return clips[m_counter++ % clips.Length];
        // }
    }
}
