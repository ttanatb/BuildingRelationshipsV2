using System;
using Dialogue.Struct;
using UnityEngine;

namespace Dialogue
{
    public class SceneActor : MonoBehaviour
    {
        [SerializeField] private ActorData m_actorData = new ActorData();
        
        private void Start()
        {
            ActorManager.Instance.Add(m_actorData.ID, m_actorData);
        }
    }
}
