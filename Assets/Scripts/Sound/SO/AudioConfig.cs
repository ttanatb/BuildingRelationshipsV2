using UnityEngine;

namespace Sound.SO
{
    [CreateAssetMenu(fileName = "AudioConfig", menuName = "br/AudioConfig", order = 1)]
    public class AudioConfig : ScriptableObject
    {
        [field: SerializeField]
        public float MasterVolume { get; set; } = 1.0f;

        [field: SerializeField]
        public float BgmVolume { get; set; } = 1.0f;

        [field: SerializeField]
        public float SfxVolume { get; set; } = 1.0f;
    }
}
