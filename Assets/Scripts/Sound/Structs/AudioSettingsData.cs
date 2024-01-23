using UnityEngine;

namespace Sound.Structs
{
    [System.Serializable]
    public struct AudioSettingsData
    {
        [field: SerializeField]
        public float MasterVolume { get; set; }

        [field: SerializeField]
        public float BgmVolume { get; set; }

        [field: SerializeField]
        public float SfxVolume { get; set; }
    }
}
