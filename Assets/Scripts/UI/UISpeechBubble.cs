using System;
using UnityEngine;
using Dialogue.SO;
using Febucci.UI.Core;
using Input.SO;
using NaughtyAttributes;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Utilr.Utility;
using Yarn.Unity;

public class UISpeechBubble : DialogueViewBase
{
    [SerializeField] private TMPro.TextMeshProUGUI m_nameText = null;
    [SerializeField] private TypewriterCore m_typewriter = null;

    [SerializeField] private InputActionReference m_triggerNextDialogueAction = null;
    [SerializeField] private SwitchInputActionMapEvent m_switchToPlayer = null;
    [SerializeField] private StopDialogueEvent m_stopDialogueEvent = null;
    
    [SerializeField] private Animator m_animator = null;
        
    [SerializeField] [AnimatorParam("m_animator")]
    private int m_showAnimTrigger = 1;

    [SerializeField] [AnimatorParam("m_animator")]
    private int m_hideAnimTrigger = 1;

    [SerializeField] private float m_durBetweenInputs = 0.3f;
    [SerializeField] private float m_hideDelay = 0.4f;

    private Action m_onCurrentLineDismissed = null;

    private RectTransform m_transform = null;
    private UIManager m_uiManager = null;
    private EventManager m_eventManager = null;
    private bool m_isShowingOptions = false;
    private float m_previousInputTimestamp = 0;


    // Start is called before the first frame update
    private void Start()
    {
        m_uiManager = UIManager.Instance;
        m_transform = GetComponent<RectTransform>();
        m_eventManager = EventManager.Instance;
        
        m_typewriter.onTextDisappeared.AddListener(OnTypewriterDisappearComplete);
        m_typewriter.onCharacterVisible.AddListener(OnTypewriterType);
        
        m_triggerNextDialogueAction.action.performed += TriggerNextDialogue;
    }


    private void OnDestroy()
    {
        m_typewriter.onTextDisappeared.RemoveListener(OnTypewriterDisappearComplete);
        m_typewriter.onCharacterVisible.RemoveListener(OnTypewriterType);

        m_triggerNextDialogueAction.action.performed -= TriggerNextDialogue;
    }


    public void TriggerNextDialogue(InputAction.CallbackContext context)
    {
        requestInterrupt.Invoke();
    }

    private void OnTypewriterType(char _)
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(m_transform);
    }

    private void OnTypewriterDisappearComplete()
    {
        StartCoroutine(Helper.ExecuteAfter(() => {
            m_onCurrentLineDismissed?.Invoke();
            m_onCurrentLineDismissed = null;
        }, m_hideDelay));
    }

    public override void DialogueStarted()
    {
        m_animator.SetTrigger(m_showAnimTrigger);
    }
    
    public override void DialogueComplete()
    {
        m_switchToPlayer.Invoke();
        m_eventManager.TriggerDialogueCompletedEvent();
        m_uiManager.ToggleInstructions("");
        m_stopDialogueEvent.Invoke();
        m_animator.SetTrigger(m_hideAnimTrigger);
    }

    public override void RunLine(LocalizedLine dialogueLine, Action onDialogueLineFinished)
    {
        if (m_isShowingOptions)
            m_animator.SetTrigger(m_showAnimTrigger);

        if (!string.IsNullOrEmpty(dialogueLine.CharacterName))
        {
            m_nameText.text = dialogueLine.CharacterName;
            LayoutRebuilder.ForceRebuildLayoutImmediate(m_nameText.GetComponent<RectTransform>());
        }

        m_typewriter.ShowText(dialogueLine.TextWithoutCharacterName.Text);
        m_typewriter.StartShowingText();
    }

    public override void InterruptLine(LocalizedLine dialogueLine, Action onDialogueLineFinished)
    {
        Debug.Log("Interrupt");
        float now = Time.time;
        if (now - m_previousInputTimestamp < m_durBetweenInputs)
            return;
        
        m_previousInputTimestamp = now;
        if (m_typewriter.isShowingText)
            m_typewriter.SkipTypewriter();
        else 
            onDialogueLineFinished.Invoke();
    }

    public override void DismissLine(Action onDismissalComplete)
    {
        m_onCurrentLineDismissed = onDismissalComplete;
        m_typewriter.StartDisappearingText();
    }

    public override void UserRequestedViewAdvancement()
    {
        requestInterrupt.Invoke();
    }

    public override void RunOptions(DialogueOption[] dialogueOptions, Action<int> onOptionSelected)
    {
        m_animator.SetTrigger(m_hideAnimTrigger);
        m_isShowingOptions = true;
    }
}
