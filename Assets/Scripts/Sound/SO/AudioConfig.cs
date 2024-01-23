using Sound.Structs;
using UnityEngine;

namespace Sound.SO
{
    [CreateAssetMenu(fileName = "AudioConfig", menuName = "br/Audio/AudioConfig", order = 1)]
    public class AudioConfig : ScriptableObject
    {
        [field: SerializeField]
        public AudioSettingsData Data { get; set; } = new AudioSettingsData()
        {
            MasterVolume = 1.0f,
            BgmVolume = 1.0f,
            SfxVolume = 1.0f,
        };
    }
}
