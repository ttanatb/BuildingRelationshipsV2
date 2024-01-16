using Cam.Events;
using Cam.Struct;
using Cinemachine;
using UnityEngine;

public class SlSetCameraTransition : MonoBehaviour
{
    [Utilr.Attributes.IncludeAllAssetsWithType]
    [SerializeField]
    private SetCamTransitionEvent[] m_camTransitionEvents = null;

    private CinemachineBrain m_brain = null;
    
    // Start is called before the first frame update
    private void Start()
    {
        TryGetComponent(out m_brain);
        
        foreach (var evt in m_camTransitionEvents)
        {
            evt.Event.AddListener(OnCamTransition);
        }    
    }

    private void OnDestroy()
    {
        foreach (var evt in m_camTransitionEvents)
        {
            evt.Event.RemoveListener(OnCamTransition);
        }    
    }

    private void OnCamTransition(CamTransitionData data)
    {
        m_brain.m_DefaultBlend = data.BlendDefinition;
    }
}
