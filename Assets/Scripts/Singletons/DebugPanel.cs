using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class DebugPanel : Singleton<DebugPanel>
{

    [SerializeField] private CanvasGroup m_canvasGroup = null;
    [SerializeField] private TextMeshProUGUI m_text = null;
    [SerializeField] private InputActionReference m_toggleDebugPanelInput = null;
    
    private StringBuilder m_stringBuilder = new StringBuilder();
    private List<MonoBehaviour> m_components = new List<MonoBehaviour>();

    private void Start()
    {
        m_toggleDebugPanelInput.action.performed += TogglePanelDisplay;
    }

    private void OnDestroy()
    {
        m_toggleDebugPanelInput.action.performed -= TogglePanelDisplay;
    }

    private void TogglePanelDisplay(InputAction.CallbackContext ctx)
    {
        m_canvasGroup.alpha = m_canvasGroup.alpha < 0.1f ? 1.0f : 0.0f;
    }

    public void Display(MonoBehaviour component)
    {
        m_components.Add(component);
    }

    // Update is called once per frame
    void Update()
    {
        m_stringBuilder.Clear();
        foreach (var component in m_components)
        {
            m_stringBuilder.AppendLine(component.ToString());
        }

        m_text.text = m_stringBuilder.ToString();
    }
}
