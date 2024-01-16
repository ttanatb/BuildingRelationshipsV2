using UnityEngine;
using Utilr.SoGameEvents;

public class SlPortraitCamera : MonoBehaviour
{
    [SerializeField] private Camera m_camera = null;
    [SerializeField]
    private MonoBehaviour[] m_components = null;
    
    [SerializeField] private SoGameEvent m_activateCam = null;
    [SerializeField] private SoGameEvent m_deactivateCam = null;

    private void Start()
    {
        m_activateCam.Event.AddListener(ActivateComponents);
        m_deactivateCam.Event.AddListener(DeactivateComponents);
    }

    private void OnDestroy()
    {
        m_activateCam.Event.RemoveListener(ActivateComponents);
        m_deactivateCam.Event.RemoveListener(DeactivateComponents);
    }

    private void ActivateComponents()
    {
        SetComponentsActive(true);
    }
    
    private void DeactivateComponents()
    {
        SetComponentsActive(false);
    }

    private void SetComponentsActive(bool isActive)
    {
        foreach (var c in m_components)
        {
            c.enabled = isActive;
        }

        if (m_camera == null) return;

        m_camera.enabled = isActive;
    }
}
