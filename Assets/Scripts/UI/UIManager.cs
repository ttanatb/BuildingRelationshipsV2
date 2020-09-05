using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    private UIInventoryController m_inventoryUI = null;

    [SerializeField]
    private StringInstructionPanelPair[] m_instructionPanels = null;

    private RectTransform m_interactTextTransform = null;

    private Transform m_interactMarker = null;

    public void DisplayFishingUI(bool toDisplay)
    {
        m_fishingController.gameObject.SetActive(toDisplay);
    }

    private Transform m_currTalker = null;
    private Camera m_mainCamera = null;

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

    public void SetCurrTalkingPerson(Transform currTalker)
    {
        m_currTalker = currTalker;
    }

    public void SetInventoryActive(bool active)
    {
        m_inventoryUI.SetDisplay(active);
    }

    // Start is called before the first frame update
    void Start()
    {
        m_mainCamera = Camera.main;
        m_interactTextTransform = m_interactText.GetComponent<RectTransform>();
        SetCurrInteractAnchor(null);
        ToggleInstructions("");
    }

    // Update is called once per frame
    void Update()
    {
        if (m_currTalker != null)
            m_speechBubble.ScreenPos =
                m_mainCamera.WorldToViewportPoint(m_currTalker.position);

        if (m_interactMarker != null)
            m_interactText.ScreenPos =
                m_mainCamera.WorldToViewportPoint(m_interactMarker.position);
    }
}
