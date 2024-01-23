using System;
using RichPresence.SO;
using RichPresence.Structs;
using Steamworks;
using UnityEngine;
using Utilr.Attributes;

namespace RichPresence
{
    public class RichPresenceManager : Utilr.Singleton<RichPresenceManager>
    {
        [SerializeField] [IncludeAllAssetsWithType] 
        private SetRichPresenceEvent[] m_setRichPresenceEvents = null;
        
        [SerializeField] [IncludeAllAssetsWithType] 
        private SetRichPresenceStringSubstitution[] m_setRichPresenceStringSubstitutions = null;

        private void Start()
        {
            foreach (var e in m_setRichPresenceEvents)
            {
                e.Event.AddListener(OnRichPresenceSet);
            }

            foreach (var e in m_setRichPresenceStringSubstitutions)
            {
                e.Event.AddListener(OnRichPresenceStringSub);
            }
        }

        private void OnDestroy()
        {
            foreach (var e in m_setRichPresenceEvents)
            {
                e.Event.RemoveListener(OnRichPresenceSet);
            }
            foreach (var e in m_setRichPresenceStringSubstitutions)
            {
                e.Event.RemoveListener(OnRichPresenceStringSub);
            }
        }
        
        private static void OnRichPresenceSet(RichPresenceData data)
        {
            #if !DISABLESTEAMWORKS
            SteamFriends.SetRichPresence("steam_display", data.Key);
            #endif
        }
        
        private static void OnRichPresenceStringSub(RichPresenceStringSubstitution data)
        {
            #if !DISABLESTEAMWORKS
            SteamFriends.SetRichPresence(data.Key, data.Value);
            #endif
        }

    }
}
