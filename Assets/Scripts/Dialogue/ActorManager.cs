using Dialogue.SerializedDict;
using UnityEngine;

namespace Dialogue
{
    /// <summary>
    /// ???
    /// </summary>
    public class ActorManager : Singleton<ActorManager>
    {
        [SerializeField] private IdToActorDict m_actorDatabase = new IdToActorDict();

        // public ActorNpc GetActor(string id)
        // {
        //     if (!m_actorDatabase.Contains(id))
        //     {
        //         Debug.LogError($"Actor database does not contain: {id}");
        //     }
        //     return m_actorDatabase[id];
        // }

        public bool Contains(string id)
        {
            return m_actorDatabase.Contains(id);
        }

        public void Add(string id, ActorNpc data)
        {
            if (m_actorDatabase.Contains(id))
            {
                Debug.LogError($"Actor database already contains id {id} with: {data}");
            }

            m_actorDatabase.Add(id, data);
        }
    }
}
