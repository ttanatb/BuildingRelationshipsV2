using System;
using Achievements.SO;
using Achievements.Structs;
using Dialogue.SerializedDict;
using Dialogue.SO;
using Dialogue.Struct;
using FlagSystem.SO;
using FlagSystem.Structs;
using Inventory.SerializedDict;
using Inventory.SO;
using Saves.SO;
using Saves.Structs;
using Settings.SO;
using Settings.Structs;
using UnityEngine;
using Utilr.Utility;

namespace Saves
{
    public class SaveManager : Utilr.Singleton<SaveManager>
    {
        [SerializeField]
        private AchievementUpdateEvent m_achievementUpdateEvent = null;

        [SerializeField]
        private SettingsUpdateEvent m_settingsUpdateEvent = null;

        [SerializeField]
        private FlagGraphUpdateEvent m_flagGraphUpdateEvent = null;
        
        [SerializeField]
        private InventoryUpdatedEvent m_inventoryUpdatedEvent = null;
        
        [SerializeField] 
        private YarnVariableUpdateEvent m_yarnVariableUpdateEvent = null;

        [SerializeField] private LoadSaveEvent m_loadSaveEvent = null;
        [SerializeField] private WriteSaveEvent m_writeSaveEvent = null;

        [SerializeField] private SaveData m_currentData = new SaveData()
        {
            YarnVariableData = new YarnVariableData()
            {
                StringDict = new StringToStringDict(),
                BoolDict = new StringToBoolDict(),
                FloatDict = new StringToFloatDict(),
            }
        };
        
        private void Start()
        {
            m_achievementUpdateEvent.Event.AddListener(OnAchievementUpdated);
            m_settingsUpdateEvent.Event.AddListener(OnSettingsUpdated);
            m_flagGraphUpdateEvent.Event.AddListener(OnFlagGraphUpdated);
            m_inventoryUpdatedEvent.Event.AddListener(OnInventoryUpdated);
            m_yarnVariableUpdateEvent.Event.AddListener(OnYarnVariableUpdated);

            StartCoroutine(Helper.ExecuteNextFrame(LoadSaveData));
            Application.quitting += OnApplicationQuit;
            m_writeSaveEvent.Event.AddListener(OnApplicationQuit);
        }
        private void OnApplicationQuit()
        {
            // TODO: write data to Application.persistentDataPath
        }

        private void LoadSaveData()
        {
            // TODO: read from Application.persistentDataPath
            m_loadSaveEvent.Invoke(m_currentData);
        }

        private void OnDestroy()
        {
            m_achievementUpdateEvent.Event.RemoveListener(OnAchievementUpdated);
            m_settingsUpdateEvent.Event.RemoveListener(OnSettingsUpdated);
            m_flagGraphUpdateEvent.Event.RemoveListener(OnFlagGraphUpdated);
            m_inventoryUpdatedEvent.Event.RemoveListener(OnInventoryUpdated);
            m_yarnVariableUpdateEvent.Event.RemoveListener(OnYarnVariableUpdated);
            
            m_writeSaveEvent.Event.RemoveListener(OnApplicationQuit);
        }
        
        private void OnAchievementUpdated(AchievementCompletionData data)
        {
        }
        private void OnSettingsUpdated(SettingsData data)
        {
        }
        private void OnFlagGraphUpdated(FlagGraphUpdateStatus data)
        {
        }
        private void OnInventoryUpdated(ItemIdToItemCountDict data)
        {
        }
        private void OnYarnVariableUpdated(YarnVariableData data)
        {
        }

    }
}
