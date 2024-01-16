using System;
using Dialogue.SO;
using Dialogue.Struct;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using Utilr.Attributes;
using Utilr.Utility;
using Yarn.Unity;

namespace UI
{
    public class UiDialoguePanel : DialogueViewBase
    {
        [SerializeField] private Animator m_animator = null;
        
        [SerializeField] [AnimatorParam("m_animator")]
        private int m_showAnimTrigger = 1;

        [SerializeField] [AnimatorParam("m_animator")]
        private int m_hideAnimTrigger = 1;

        private UiDialogueOption[] m_buttons = null;
        RectTransform m_transform = null;

        [SerializeField] private TMPro.TextMeshProUGUI m_dialogueText = null;
        [SerializeField] private TMPro.TextMeshProUGUI m_nameText = null;
        [SerializeField] private float m_hideDelay = 1.0f;
        [SerializeField] private float m_inputDelay = 0.8f;

        [IncludeAllAssetsWithType]
        [SerializeField] private StartDialogueEvent[] m_startDialogueEvents = null;
        
        private void Start()
        {
            foreach (var e in m_startDialogueEvents)
            {
                e.Event.AddListener(StartDialogue);
            }

            TryGetComponent(out m_transform);
            m_buttons = GetComponentsInChildren<UiDialogueOption>();
        }

        private void OnDestroy()
        {
            foreach (var e in m_startDialogueEvents)
            {
                e.Event.RemoveListener(StartDialogue);
            }
        }
        
        private void StartDialogue(StartDialogueData _)
        {
            
        }

        private void HideOtherButtons(int index)
        {
            Assert.IsTrue(index >= 0 && index < m_buttons.Length);
            for (int i = 0; i < m_buttons.Length; i++)
            {
                m_buttons[i].DisableButton();
                if (i == index) continue;
                
                m_buttons[i].DisappearText();
            }
            

            StartCoroutine(Helper.ExecuteAfter(() => {
                m_animator.SetTrigger(m_hideAnimTrigger);
            }, m_hideDelay));
        }

        public override void RunLine(LocalizedLine dialogueLine, Action onDialogueLineFinished)
        {
            if (!string.IsNullOrEmpty(dialogueLine.CharacterName))
            {
                m_nameText.text = dialogueLine.CharacterName;
            }
            m_dialogueText.text = dialogueLine.TextWithoutCharacterName.Text;
            onDialogueLineFinished?.Invoke();
        }

        public override void RunOptions(DialogueOption[] dialogueOptions, Action<int> onOptionSelected)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(m_transform);
            m_animator.SetTrigger(m_showAnimTrigger);
            
            foreach (var button in m_buttons)
                button.SetVisible(false);

            int buttonIndex = 0;
            for (int i = 0; i < dialogueOptions.Length; i++)
            {
                var option = dialogueOptions[i];
                if (!option.IsAvailable) continue;
                
                m_buttons[buttonIndex].SetVisible(true);
                m_buttons[buttonIndex].Init(dialogueOptions[i], onOptionSelected, HideOtherButtons, i);
                buttonIndex++;
            }
            
            StartCoroutine(Helper.ExecuteAfter(() => {
                m_buttons[0].SetSelected();
            }, m_inputDelay));
        }
    }
}
