using FlagSystem.SO;
using NaughtyAttributes;
using UnityEngine;

namespace FlagSystem
{
    public class FlagGraphManager : Singleton<FlagGraphManager>
    {
        [Expandable] [SerializeField] private FlagGraph m_graph = null;
        [Expandable] [SerializeField] private FlagCompletionEvent m_flagCompletionEvent = null;

        private void Start()
        {
            m_flagCompletionEvent.Event.AddListener(m_graph.OnFlagCompleted);
        }

        private void OnDestroy()
        {
            m_flagCompletionEvent.Event.RemoveListener(m_graph.OnFlagCompleted);
        }
    }
}
