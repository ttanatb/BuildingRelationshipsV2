using Dialogue.SO;
using Dialogue.Struct;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Light))]
public class SlEnableSpotLight : MonoBehaviour
{
    [Utilr.Attributes.IncludeAllAssetsWithType]
    [SerializeField] private StartDialogueEvent[] m_enableLights = null;
    [SerializeField] private StopDialogueEvent m_disableLight = null;

    private Light m_light = null;

    private void Start()
    {
        foreach (var e in m_enableLights)
        {
            e.Event.AddListener(OnStartDialogue);
        }
        m_disableLight.Event.AddListener(OnStopDialogue);

        TryGetComponent(out m_light);
    }

    private void OnDestroy()
    {
        foreach (var e in m_enableLights)
        {
            e.Event.RemoveListener(OnStartDialogue);
        }
        m_disableLight.Event.RemoveListener(OnStopDialogue);
    }

    private void OnStopDialogue()
    {
        m_light.enabled = false;
    }

    private void OnStartDialogue(StartDialogueData _)
    {
        m_light.enabled = true;
    }
}
