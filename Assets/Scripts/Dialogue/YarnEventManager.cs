using System;
using System.Collections;
using Cinemachine;
using Dialogue.SO;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Assertions;
using Yarn.Unity;

namespace Dialogue
{
    public class YarnEventManager : Singleton<YarnEventManager>
    {
        /// <summary>
        /// Table for re-routing generic game events.
        /// </summary>
        [Expandable] [SerializeField] private YarnEventDb m_eventDb = null;

        /// <summary>
        /// Brain mainly used for camera transitions.
        /// </summary>
        [SerializeField] private CinemachineBrain m_brain = null;
        
        private void Start()
        {
            // It's because Yarn indexes for things based on game object name :(
            gameObject.name = "YarnEventManager";
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
        
        [YarnCommand("setActorAlias")]
        private void SetActorAlias(string id, string displayName)
        {
            // idk about this one yet
        }
    }
}
