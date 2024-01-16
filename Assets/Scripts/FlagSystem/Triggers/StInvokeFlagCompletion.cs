using FlagSystem.SO;
using NaughtyAttributes;
using UnityEngine;

namespace FlagSystem.Triggers
{
    /// <summary>
    /// Used by other MonoBehaviours to invoke flag completion
    /// </summary>
    public class StInvokeFlagCompletion : MonoBehaviour
    {
        [SerializeField]
        private FlagCompletionEvent m_event = null;

        [SerializeField]
        private FlagNode m_node = null;
        
        [Button]
        public void Invoke()
        {
            m_event.Invoke(m_node);
        }
    }
}
