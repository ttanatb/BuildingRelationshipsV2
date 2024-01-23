using FlagSystem.SO;
using NaughtyAttributes;
using Saves.SO;
using Saves.Structs;
using UnityEngine;

namespace FlagSystem
{
    public class FlagGraphManager : Singleton<FlagGraphManager>
    {
        [Expandable] [SerializeField] private FlagGraph m_graph = null;
        [SerializeField] private FlagCompletionEvent m_flagCompletionEvent = null;

        [SerializeField] private LoadSaveEvent m_onSaveDataLoad = null;
        
        private void Start()
        {
            m_flagCompletionEvent.Event.AddListener(m_graph.OnFlagCompleted);
            m_onSaveDataLoad.Event.AddListener(OnSaveDataLoaded);
        }

        private void OnDestroy()
        {
            m_flagCompletionEvent.Event.RemoveListener(m_graph.OnFlagCompleted);
            m_onSaveDataLoad.Event.RemoveListener(OnSaveDataLoaded);
        }

        private void OnSaveDataLoaded(SaveData data)
        {
            m_graph.LoadCompletionStatus(data.FlagCompletionStatus);
        }
    }
}
