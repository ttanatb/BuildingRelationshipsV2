using System;
using System.Text;
using Input.SO;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using Utilr.Attributes;

namespace Input
{
    public class InputManager : Singleton<InputManager>
    {
        [SerializeField] private InputActionReference m_playerInput = null;
        [SerializeField] private InputActionReference m_fishInput = null;
        [SerializeField] private InputActionReference m_uiInput = null;
        [SerializeField] private InputActionReference[] m_alwaysActive = null;

        [IncludeAllAssetsWithType]
        [SerializeField] private SwitchInputActionMapEvent[] m_switchInputActionMapEvents = null;

        [SerializeField] private InputActionReference m_startingInputMap = null;

        private void Start()
        {
            SwitchCurrentActionMap(m_startingInputMap);

            foreach (var evt in m_switchInputActionMapEvents)
            {
                evt.Event.AddListener(SwitchCurrentActionMap);
            }

            foreach (var reference in m_alwaysActive)
            {
                reference.action.actionMap.Enable();
            }
            
            DebugPanel.Instance.Display(this);
        }

        private void OnDestroy()
        {
            foreach (var evt in m_switchInputActionMapEvents)
            {
                evt.Event.RemoveListener(SwitchCurrentActionMap);
            }
        }
        

        private void SwitchCurrentActionMap(InputActionReference input)
        {
            m_playerInput.action.actionMap.Disable();
            m_uiInput.action.actionMap.Disable();
            m_fishInput.action.actionMap.Disable();
            
            input.action.actionMap.Enable();
        }

        public override string ToString()
        {
            return
                $"Player ({m_playerInput.action.actionMap.enabled}), " +
                $"Fishing ({m_fishInput.action.actionMap.enabled}), " + 
                $"UI ({m_uiInput.action.actionMap.enabled})";
        }
    }
}
