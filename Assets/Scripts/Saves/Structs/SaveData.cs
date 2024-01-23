using Achievements.Structs;
using Dialogue.Struct;
using FlagSystem.Structs;
using Inventory.SerializedDict;
using Settings.Structs;
using UnityEngine;

namespace Saves.Structs
{
    [System.Serializable]
    public struct SaveData
    {
        [field: SerializeField]
        public SettingsData Settings { get; set; }
        
        [field: SerializeField]
        public AchievementCompletionData Achievement { get; set; }
        
        [field: SerializeField]
        public FlagGraphUpdateStatus FlagCompletionStatus { get; set; }
        
        [field: SerializeField]
        public ItemIdToItemCountDict Inventory { get; set; }
        
        [field: SerializeField]
        public YarnVariableData YarnVariableData { get; set; }
    }
}
