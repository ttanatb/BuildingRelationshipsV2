using System;
using System.Collections;
using System.Collections.Generic;
using Dialogue.SO;
using Dialogue.Struct;
using UnityEngine;
using UnityEngine.UI;
using Utilr.Attributes;

[Serializable]
public struct StringInstructionPanelPair
{
    public string Name;
    public GameObject Panel;
}

public class UIManager : Singleton<UIManager>
{
    [SerializeField]
    private UISpeechBubble m_speechBubble = null;

    [SerializeField]
    private UIInteractText m_interactText = null;

    [SerializeField]
    private UIFishingController m_fishingController = null;

    [SerializeField]
    private StringInstructionPanelPair[] m_instructionPanels = null;

    private RectTransform m_interactTextTransform = null;

    private Transform m_interactMarker = null;

    private Transform m_currTalker = null;
    private Camera m_mainCamera = null;
    
    [IncludeAllAssetsWithType]
    [SerializeField] private StartDialogueEvent[] m_startDialogueEvents = null;
    [SerializeField] private StopDialogueEvent m_stopDialogueEvent = null;

    public UIFishingController FishingControllerUI { get { return m_fishingController; } }

    public void ToggleInstructions(string name)
    {
        bool found = false;
        foreach (var pair in m_instructionPanels)
        {
            if (pair.Name == name)
            {
                found = true;
                pair.Panel.SetActive(true);
            }
            else
            {
                pair.Panel.SetActive(false);
            }
        }

        if (name != "" && !found)
            Debug.LogError("Could not find instruction panel named: " + name);
    }

    public void SetCurrInteractAnchor(Transform currObj, string message = "interact")
    {
        m_interactMarker = currObj;
        if (m_interactMarker == null)
        {
            m_interactText.gameObject.SetActive(false);
        }
        else
        {
            m_interactText.gameObject.SetActive(true);
            m_interactText.Text = message;
            LayoutRebuilder.ForceRebuildLayoutImmediate(m_interactTextTransform);
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
        m_mainCamera = Camera.main;
        m_interactTextTransform = m_interactText.GetComponent<RectTransform>();
        SetCurrInteractAnchor(null);
        ToggleInstructions("");
        
        foreach (var e in m_startDialogueEvents)
        {
            e.Event.AddListener(StartDialogue);
        }
        m_stopDialogueEvent.Event.AddListener(StopDialogue);
    }

    private void OnDestroy()
    {
        foreach (var e in m_startDialogueEvents)
        {
            e.Event.RemoveListener(StartDialogue);
        }
        m_stopDialogueEvent.Event.RemoveListener(StopDialogue);
    }
    
    private void StartDialogue(StartDialogueData data)
    {
        // m_currTalker = data.DialogueBoxAnchor;
        ToggleInstructions("Dialogue");
    }
    
    private void StopDialogue()
    {
        m_currTalker = null;
    }

    // Update is called once per frame
    private void Update()
    {
        // if (m_currTalker != null)
            // m_speechBubble.ScreenPos =
                // m_mainCamera.WorldToViewportPoint(m_currTalker.position);

        if (m_interactMarker != null)
            m_interactText.ScreenPos =
                m_mainCamera.WorldToViewportPoint(m_interactMarker.position);
    }
}
