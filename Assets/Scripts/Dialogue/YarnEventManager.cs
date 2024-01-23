using System;
using System.Collections;
using Cinemachine;
using Dialogue.SO;
using Dialogue.Struct;
using Inventory.SO;
using Inventory.Structs;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Assertions;
using Yarn.Unity;

namespace Dialogue
{
    public class YarnEventManager : Singleton<YarnEventManager>
    {
        [SerializeField] private SetActiveNodeEvent m_setActiveNodeEvent = null;
        [SerializeField] private MoveActorEvent m_moveActorEvent = null;
        [SerializeField] private SetCurrentActorIdEvent m_setCurrentActorIdEvent = null;
        [SerializeField] private ObtainItemEvent m_obtainItemEvent = null;
            
        /// <summary>
        /// Table for re-routing generic game events.
        /// </summary>
        [Expandable] [SerializeField] private YarnEventDb m_eventDb = null;

        /// <summary>
        /// Brain mainly used for camera transitions.
        /// </summary>
        [SerializeField] private CinemachineBrain m_brain = null;

        private ActorManager m_actorManager = null;
        
        private void Start()
        {
            // It's because Yarn indexes for things based on game object name :(
            gameObject.name = "YarnEventManager";
            
            m_actorManager = ActorManager.Instance;
        }

        [YarnCommand("fireEvent")]
        private void FireEvent(string eventName)
        {
            var dict = m_eventDb.YarnEventNameToGameEventDict;
            Assert.IsTrue(dict.ContainsKey(eventName));
            Assert.IsNotNull(dict[eventName]);
            dict[eventName].Invoke();
        }
        
        [YarnCommand("blockOnCameraMovement")]
        private IEnumerator BlockOnCameraMovement()
        {
            yield return new WaitForSeconds(m_brain.m_DefaultBlend.BlendTime);
        }
        
        [YarnCommand("setActiveNode")]
        private void SetActiveNode(string id, string nodeName) 
        {
            if (!m_actorManager.Contains(id))
                Debug.LogError($"[setActiveNode({id}, {nodeName})]: Actor with id {id} does not exist");
            
            m_setActiveNodeEvent.Invoke(new SetActiveNodeData()
            {
                ActorId = id,
                NodeName = nodeName,
            });
        }
        
        [YarnCommand("moveActor")]
        private void MoveActor(string id, string posId) 
        {
            if (!m_actorManager.Contains(id))
                Debug.LogError($"[moveActor({id}, {posId})]: Actor with id {id} does not exist");

            m_moveActorEvent.Invoke(new MoveActorData()
            {
                ActorId = id,
                PositionId = posId,
            });
        }
        
        [YarnCommand("setCurrentActorId")]
        private void SetCurrentActorId(string id) 
        {
            if (!m_actorManager.Contains(id))
                Debug.LogError($"[setCurrentActorId({id})]: Actor with id {id} does not exist");

            m_setCurrentActorIdEvent.Invoke(id);
        }
        
                
        [YarnCommand("obtainItem")]
        private void ObtainItem(string enumString, int amount)
        {
            if (!Enum.TryParse(enumString, out ItemData.ItemID id))
            {
                Debug.LogError($"[ObtainItem({enumString}, {amount})]: " +
                    $"Enum {enumString} does not exist");
            }
            
            m_obtainItemEvent.Invoke(new ItemCount()
            {
                Id = id,
                Count = amount,
            });
        }
    }
}
