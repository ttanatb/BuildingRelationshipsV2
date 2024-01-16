using FlagSystem.SO;
using NaughtyAttributes;
using UnityEngine;

namespace FlagSystem.Listeners
{
    public class SlSetAnimTriggerAfterFlag : MonoBehaviour
    {
        [SerializeField] private FlagCompletionEvent m_event = null;
        [SerializeField] private FlagNode m_nodeToListenFor = null;
        [SerializeField] private Animator m_animator = null;
        
        [AnimatorParam("m_animator")] [SerializeField]
        private int m_animTrigger = 0;
        
        private void Start()
        {
            m_event.Event.AddListener(OnFlagCompleted);
        }

        private void OnFlagCompleted(FlagNode node)
        {
            if (node != m_nodeToListenFor) return;

            m_animator.SetTrigger(m_animTrigger);
        }
    }
}
