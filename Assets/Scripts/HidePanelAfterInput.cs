using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;
using Utilr.SoGameEvents;

public class HidePanelAfterInput : MonoBehaviour
{
    [SerializeField] private InputActionReference m_actionReference = null;
    [SerializeField] private int m_performCount = 5;
    [SerializeField] private SoGameEvent m_eventToStartCounting = null;
    [SerializeField] private SoGameEventBase[] m_eventToTrigger = null;

    [ShowNonSerializedField]
    private int m_counter = 0;
    [ShowNonSerializedField]
    private bool m_counterActive = false;
    
    // Start is called before the first frame update
    private void Start()
    {
        m_actionReference.action.performed += OnInputAction;
        m_eventToStartCounting.Event.AddListener(OnStartCount);
    }

    private void OnDestroy()
    {
        m_actionReference.action.performed -= OnInputAction;
        m_eventToStartCounting.Event.RemoveListener(OnStartCount);
    }
    
    private void OnStartCount()
    {
        m_counter = 0;
        m_counterActive = true;
    }
    
    private void OnInputAction(InputAction.CallbackContext _)
    {
        if (!m_counterActive)
            return;
        
        m_counter++;
        if (m_counter < m_performCount)
            return;

        foreach (var evt in m_eventToTrigger)
        {
            evt.Invoke();
        }

        m_counterActive = false;
    }
}
