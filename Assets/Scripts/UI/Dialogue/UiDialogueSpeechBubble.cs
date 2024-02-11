using System;
using Dialogue;
using UnityEngine;
using Dialogue.SO;
using Dialogue.Struct;
using Febucci.UI.Core;
using Input.SO;
using NaughtyAttributes;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Utilr.SoGameEvents;
using Utilr.Utility;
using Yarn.Unity;

public class UiDialogueSpeechBubble : DialogueViewBase
{
    [SerializeField] private TMPro.TextMeshProUGUI m_nameText = null;
    [SerializeField] private TypewriterCore m_typewriter = null;

    [SerializeField] private InputActionReference m_triggerNextDialogueAction = null;
    [SerializeField] private SwitchInputActionMapEvent m_switchToPlayer = null;
    [SerializeField] private StopDialogueEvent m_stopDialogueEvent = null;
    [SerializeField] private ActorAnimationEvent m_actorAnimationEvent = null;
    [SerializeField] private SetCurrentActorIdEvent m_setCurrentActorIdEvent = null;
    [SerializeField] private SoGameEvent m_hideSpeechBubble = null;
    [SerializeField] private SoGameEvent m_showSpeechBubble = null;
    
    [SerializeField] private Animator m_animator = null;
        
    [SerializeField] [AnimatorParam("m_animator")]
    private int m_showAnimTrigger = 1;

    [SerializeField] [AnimatorParam("m_animator")]
    private int m_hideAnimTrigger = 1;

    [SerializeField] private float m_durBetweenInputs = 0.3f;
    [SerializeField] private float m_hideDelay = 0.4f;

    private Action m_onCurrentLineDismissed = null;

    private string m_currActorId = "";
    private RectTransform m_transform = null;
    private UIManager m_uiManager = null;
    private bool m_isShowingOptions = false;
    private float m_previousInputTimestamp = 0;


    // Start is called before the first frame update
    private void Start()
    {
        m_uiManager = UIManager.Instance;
        m_transform = GetComponent<RectTransform>();
        
        m_typewriter.onTextDisappeared.AddListener(OnTypewriterDisappearComplete);
        m_typewriter.onCharacterVisible.AddListener(OnTypewriterType);
        m_typewriter.onTextShowed.AddListener(OnTypewriterComplete);

        m_setCurrentActorIdEvent.Event.AddListener(OnSetCurrentActorId);
        m_hideSpeechBubble.Event.AddListener(OnHideSpeechBubble);
        m_showSpeechBubble.Event.AddListener(OnShowSpeechBubble);
        
        m_triggerNextDialogueAction.action.performed += TriggerNextDialogue;
    }

    private void OnDestroy()
    {
        m_typewriter.onTextDisappeared.RemoveListener(OnTypewriterDisappearComplete);
        m_typewriter.onCharacterVisible.RemoveListener(OnTypewriterType);
        m_typewriter.onTextShowed.RemoveListener(OnTypewriterComplete);
        
        m_setCurrentActorIdEvent.Event.RemoveListener(OnSetCurrentActorId);
        m_hideSpeechBubble.Event.RemoveListener(OnHideSpeechBubble);
        m_showSpeechBubble.Event.RemoveListener(OnShowSpeechBubble);

        m_triggerNextDialogueAction.action.performed -= TriggerNextDialogue;
    }
    

    private void OnTypewriterType(char _)
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(m_transform);
    }
    
    private void OnTypewriterComplete()
    {
        m_actorAnimationEvent.Invoke(new ActorAnimationData()
        {
            ActorId = m_currActorId,
            TriggerName = ActorAnimationData.AnimTrigger.StopTalk,
        });   
    }

    private void OnTypewriterDisappearComplete()
    {
        StartCoroutine(Helper.ExecuteAfter(() => {
            m_onCurrentLineDismissed?.Invoke();
            m_onCurrentLineDismissed = null;
        }, m_hideDelay));
    }

    private void OnSetCurrentActorId(string id)
    {
        m_currActorId = id;
    }
    
    private void OnHideSpeechBubble()
    {
        m_animator.SetTrigger(m_hideAnimTrigger);
    }
    
    private void OnShowSpeechBubble()
    {
        m_animator.SetTrigger(m_showAnimTrigger);
    }


    public void TriggerNextDialogue(InputAction.CallbackContext context)
    {
        requestInterrupt.Invoke();
    }
    
    public override void DialogueStarted()
    {
        m_animator.SetTrigger(m_showAnimTrigger);
    }
    
    public override void DialogueComplete()
    {
        m_switchToPlayer.Invoke();
        m_uiManager.ToggleInstructions("");
        m_stopDialogueEvent.Invoke();
        m_animator.SetTrigger(m_hideAnimTrigger);
    }

    public override void RunLine(LocalizedLine dialogueLine, Action onDialogueLineFinished)
    {
        if (m_isShowingOptions)
        {
            m_animator.SetTrigger(m_showAnimTrigger);
            m_isShowingOptions = false;
        }

        if (!string.IsNullOrEmpty(dialogueLine.CharacterName))
        {
            m_nameText.text = dialogueLine.CharacterName;
            LayoutRebuilder.ForceRebuildLayoutImmediate(m_nameText.GetComponent<RectTransform>());
        }

        m_actorAnimationEvent.Invoke(new ActorAnimationData()
        {
            ActorId = m_currActorId,
            TriggerName = ActorAnimationData.AnimTrigger.StartTalk,
        });  
        m_typewriter.ShowText(dialogueLine.TextWithoutCharacterName.Text);
        m_typewriter.StartShowingText();
    }

    public override void InterruptLine(LocalizedLine dialogueLine, Action onDialogueLineFinished)
    {
        // Debug.Log("Interrupt");
        float now = Time.time;
        if (now - m_previousInputTimestamp < m_durBetweenInputs)
            return;
        
        m_previousInputTimestamp = now;
        if (m_typewriter.isShowingText)
        {
            m_typewriter.SkipTypewriter();

        }
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
