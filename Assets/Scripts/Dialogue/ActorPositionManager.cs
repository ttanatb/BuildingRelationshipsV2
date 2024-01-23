using System.Collections.Generic;
using UnityEngine;

namespace Dialogue
{
    public class ActorPositionManager : Singleton<ActorPositionManager>
    {
        private readonly Dictionary<string, ActorPositionAnchor> m_dict 
            = new Dictionary<string, ActorPositionAnchor>();
        
        public void Register(string id, ActorPositionAnchor anchor)
        {
            if (!m_dict.TryAdd(id, anchor))
                Debug.LogError($"Trying to register ({id}: {anchor.gameObject}) when " +
                    $"({id}: {m_dict[id].gameObject}) already exists");
        }

        public ActorPositionAnchor Get(string id)
        {
            return m_dict[id];
        }
    }
}
