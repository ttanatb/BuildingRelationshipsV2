using Dialogue.SerializedDict;
using Dialogue.Struct;
using UnityEngine;

namespace Dialogue
{
    public class ActorManager : Singleton<ActorManager>
    {
        [SerializeField] private IdToActorDataDict m_actorDatabase = null;

        public ActorData GetActor(string id)
        {
            if (!m_actorDatabase.Contains(id))
            {
                Debug.LogError($"Actor database does not contain: {id}");
            }
            return m_actorDatabase[id];
        }

        public bool Contains(string id)
        {
            return m_actorDatabase.Contains(id);
        }

        public void Add(string id, ActorData data)
        {
            if (m_actorDatabase.Contains(id))
            {
                Debug.LogError($"Actor database already contains id {id} with: {data}");
            }

            m_actorDatabase.Add(id, data);
        }
    }
}
