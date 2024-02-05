using System;
using Dialogue.SO;
using Dialogue.Struct;
using UnityEngine;
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
    private UIFishingController m_fishingController = null;

    [SerializeField]
    private StringInstructionPanelPair[] m_instructionPanels = null;

    [IncludeAllAssetsWithType]
    [SerializeField] private StartDialogueEvent[] m_startDialogueEvents = null;

    public UIFishingController FishingControllerUI => m_fishingController;

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

    // Start is called before the first frame update
    private void Start()
    {
        ToggleInstructions("");
        
        foreach (var e in m_startDialogueEvents)
        {
            e.Event.AddListener(StartDialogue);
        }
    }

    private void OnDestroy()
    {
        foreach (var e in m_startDialogueEvents)
        {
            e.Event.RemoveListener(StartDialogue);
        }
    }
    
    private void StartDialogue(StartDialogueData data)
    {
        ToggleInstructions("Dialogue");
    }
}
