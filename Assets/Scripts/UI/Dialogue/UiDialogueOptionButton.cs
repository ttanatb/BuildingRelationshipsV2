using System;
using Febucci.UI.Core;
using UnityEngine;
using UnityEngine.UI;
using Utilr.Utility;
using Yarn.Unity;

namespace UI
{
    public class UiDialogueOptionButton : MonoBehaviour 
    {
        private CanvasGroup m_canvasGroup = null;
        private Button m_button;
        [SerializeField] private TypewriterCore m_typewriter = null;
        [SerializeField] private float m_advanceDialogueDelay = 1.0f;

        private RectTransform m_parent = null;
        
        private void Start()
        {
            TryGetComponent(out m_canvasGroup);
            m_button = GetComponentInChildren<Button>();
            
            m_typewriter.onCharacterVisible.AddListener(OnTypewriterType);
            m_parent = transform.parent.GetComponent<RectTransform>();
        }


        private void OnDestroy()
        {
            m_typewriter.onCharacterVisible.RemoveListener(OnTypewriterType);
        }

        private void OnTypewriterType(char _)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(m_parent);
        }

        public void SetSelected()
        {
            m_button.Select();
        }

        public void DisappearText()
        {
            m_typewriter.StartDisappearingText();
        }

        public void DisableButton()
        {
            m_button.interactable = false;
            m_button.onClick.RemoveAllListeners();
        }

        public void SetVisible(bool isVisible)
        {
            m_canvasGroup.alpha = isVisible ? 1 : 0;
            m_canvasGroup.interactable = isVisible;
        }

        public void Init(DialogueOption dialogueOption, Action<int> onOptionSelected, Action<int> hideOtherButtonsCb, int index)
        {
            m_button.interactable = true;
            m_button.onClick.AddListener(() => {
                hideOtherButtonsCb.Invoke(index);

                StartCoroutine(Helper.ExecuteAfter(() => {
                    onOptionSelected.Invoke(index);
                }, m_advanceDialogueDelay));
            });
            
            m_typewriter.ShowText(dialogueOption.Line.Text.Text);
            m_typewriter.StartShowingText();
        }
    }
}
