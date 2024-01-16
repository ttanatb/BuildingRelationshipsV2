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
        [IncludeAllAssetsWithType]
        [SerializeField] private StartDialogueEvent[] m_startDialogueEvents = null;
        [SerializeField] private DialogueRunner m_dialogueRunner = null;
        [SerializeField] private SwitchInputActionMapEvent m_switchToUiEvent = null;

        [SerializeField] private SoGameEventGroup m_onDialogueComplete = null;

        
        private void Start()
        {
            foreach (var e in m_startDialogueEvents)
            {
                e.Event.AddListener(StartDialogue);
            }
            
            m_dialogueRunner.onDialogueComplete.AddListener(OnDialogueComplete);
        }

        private void OnDestroy()
        {
            foreach (var e in m_startDialogueEvents)
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
            m_switchToUiEvent.Invoke();
        }
        
    }
}
