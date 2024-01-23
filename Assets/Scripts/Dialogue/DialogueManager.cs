using Dialogue.SO;
using Dialogue.Struct;
using Input.SO;
using UnityEngine;
using UnityEngine.Serialization;
using Utilr.Attributes;
using Utilr.SoGameEvents;
using Yarn.Unity;

namespace Dialogue
{
    public class DialogueManager : Singleton<DialogueManager>
    {
        [FormerlySerializedAs("m_startDialogueEvents")]
        [IncludeAllAssetsWithType]
        [SerializeField] private StartDialogueEvent[] m_startDialogueEventsToListenTo = null;
        [SerializeField] private DialogueRunner m_dialogueRunner = null;

        [SerializeField] private SoGameEventGroup m_eventToTriggerOnStart = null;

        [SerializeField] private SoGameEventGroup m_onDialogueComplete = null;

        
        private void Start()
        {
            foreach (var e in m_startDialogueEventsToListenTo)
            {
                e.Event.AddListener(StartDialogue);
            }
            
            m_dialogueRunner.onDialogueComplete.AddListener(OnDialogueComplete);
        }

        private void OnDestroy()
        {
            foreach (var e in m_startDialogueEventsToListenTo)
            {
                e.Event.RemoveListener(StartDialogue);
            }
            
            m_dialogueRunner.onDialogueComplete.RemoveListener(OnDialogueComplete);
        }

        private void OnDialogueComplete()
        {
            m_onDialogueComplete.Invoke();
        }

        private void StartDialogue(StartDialogueData data)
        {
            m_dialogueRunner.StartDialogue(data.NodeName);
            m_eventToTriggerOnStart.Invoke();
        }
    }
}
