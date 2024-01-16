using FlagSystem.SO;
using UnityEngine;

namespace FlagSystem.Listeners
{
    public class SlSetActiveAfterFlag : MonoBehaviour
    {
        [SerializeField] private FlagCompletionEvent m_event = null;
        [SerializeField] private FlagNode m_nodeToListenFor = null;

        private void Start()
        {
            m_event.Event.AddListener(OnFlagCompleted);
        }

        private void OnFlagCompleted(FlagNode node)
        {
            if (node != m_nodeToListenFor) return;
            
            gameObject.SetActive(true);
        }
    }
}
