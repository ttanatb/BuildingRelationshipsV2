using Achievements.SO;
using Achievements.Structs;
using Saves.SO;
using Saves.Structs;
using Steamworks;
using UnityEngine;
using UnityEngine.Serialization;
using Utilr.Attributes;

namespace Achievements
{
    public class AchievementManager : Utilr.Singleton<AchievementManager>
    {
        [IncludeAllAssetsWithType] [SerializeField]
        private SetAchievementFlagEvent[] m_achievementFlagEvents = null;

        [IncludeAllAssetsWithType] [SerializeField]
        private SetUserStatEvent[] m_userStatEvents = null;

        [IncludeAllAssetsWithType] [SerializeField]
        private UpdateUserStatAvgRateEvent[] m_userStatAvgRateEvents = null;

        private AchievementCompletionData m_achievementCompletionData = new AchievementCompletionData();
        
        [SerializeField] private LoadSaveEvent m_loadSaveEvent = null;
        [SerializeField] private AchievementUpdateEvent m_achievementUpdateEvent = null;
        
        private void Start()
        {
            foreach (var e in m_achievementFlagEvents)
                e.Event.AddListener(OnAchievementFlag);

            foreach (var e in m_userStatEvents)
                e.Event.AddListener(OnUserStat);

            foreach (var e in m_userStatAvgRateEvents)
                e.Event.AddListener(OnUserStatAvgRate);

            m_loadSaveEvent.Event.AddListener(OnSaveDataLoaded);
        }

        private void OnDestroy()
        {
            foreach (var e in m_achievementFlagEvents)
                e.Event.RemoveListener(OnAchievementFlag);

            foreach (var e in m_userStatEvents)
                e.Event.RemoveListener(OnUserStat);

            foreach (var e in m_userStatAvgRateEvents)
                e.Event.RemoveListener(OnUserStatAvgRate);
            
            m_loadSaveEvent.Event.RemoveListener(OnSaveDataLoaded);
        }
        
        private void OnSaveDataLoaded(SaveData data)
        {
            m_achievementCompletionData = data.Achievement;
        }

        private void OnUserStatAvgRate(UpdateUserStatAvgRateData data)
        {
            #if !DISABLESTEAMWORKS
            SteamUserStats.UpdateAvgRateStat(data.ApiName, data.Value, data.Duration);
            SteamUserStats.StoreStats();
            #endif
            
            // TODO: figure out goal value for non-steam platforms
            
            m_achievementUpdateEvent.Invoke(m_achievementCompletionData);
        }

        private void OnUserStat(SetUserStatData data)
        {
            #if !DISABLESTEAMWORKS
            SteamUserStats.SetStat(data.ApiName, data.Value);
            SteamUserStats.StoreStats();
            #endif
            
            // TODO: figure out goal value for non-steam platforms
            
            m_achievementUpdateEvent.Invoke(m_achievementCompletionData);
        }

        private void OnAchievementFlag(SetAchievementFlagData data)
        {
            #if !DISABLESTEAMWORKS
            SteamUserStats.SetAchievement(data.ApiName);
            SteamUserStats.StoreStats();
            #endif

            var completionStatus = m_achievementCompletionData.AchievementDict[data.ApiName];
            completionStatus.IsCompleted = true;
            m_achievementCompletionData.AchievementDict[data.ApiName] = completionStatus;
            
            m_achievementUpdateEvent.Invoke(m_achievementCompletionData);
        }
    }
}
