using System;
using Dialogue.SO;
using UnityEngine;
using UnityEngine.Assertions;

namespace Dialogue
{
    [RequireComponent(typeof(Collider))]
    public class SlToggleTriggerActive : MonoBehaviour
    {
        [SerializeField]
        private EnableTriggerEvent m_enableTriggerEvent = null;

        [SerializeField]
        private DisableTriggerEvent m_disableTriggerEvent = null;

        private Collider m_collider = null;

        private void Start()
        {
            m_enableTriggerEvent.Event.AddListener(OnEnableTrigger);
            m_disableTriggerEvent.Event.AddListener(OnDisableTrigger);

            TryGetComponent(out m_collider);
            Assert.IsTrue(m_collider.isTrigger);
        }

        private void OnDestroy()
        {
            m_enableTriggerEvent.Event.RemoveListener(OnEnableTrigger);
            m_disableTriggerEvent.Event.RemoveListener(OnDisableTrigger);
        }
        private void OnDisableTrigger()
        {
            m_collider.enabled = false;
        }

        private void OnEnableTrigger()
        {
            m_collider.enabled = true;
        }
    }
}
