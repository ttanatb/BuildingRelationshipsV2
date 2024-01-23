using Sound.Structs;
using UnityEngine;

namespace Settings.Structs
{
    [System.Serializable]
    public struct SettingsData
    {
        [field: SerializeField]
        public float TextSizeModifier { get; set; }
        
        [field: SerializeField]
        public AudioSettingsData AudioSettingsData { get; set; }
    }
}
