using System;
using System.Text;
using Input.SO;
using Input.Structs;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.Serialization;
using Utilr.Attributes;
using Utilr.Utility;

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

        [IncludeAllAssetsWithType]
        [SerializeField] private TriggerGamepadRumbleEvent[] m_triggerGamepadRumbleEvents = null;
        [SerializeField] private SetCurrentInputDeviceEvent m_currentInputDeviceEvent = null;
        private SetCurrentInputDeviceData.DeviceType m_previousDeviceClass = SetCurrentInputDeviceData.DeviceType.Invalid;

        private void Start()
        {
            SwitchCurrentActionMap(m_startingInputMap);
            InputSystem.onAnyButtonPress.Call(OnAnyButtonPress);
            InputSystem.onDeviceChange += OnDeviceChange;
            

            foreach (var evt in m_switchInputActionMapEvents)
            {
                evt.Event.AddListener(SwitchCurrentActionMap);
            }
            
            foreach (var evt in m_triggerGamepadRumbleEvents)
            {
                evt.Event.AddListener(OnTriggerGamepadRumble);
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
            
            foreach (var evt in m_triggerGamepadRumbleEvents)
            {
                evt.Event.RemoveListener(OnTriggerGamepadRumble);
            }
        }
        
        private void OnTriggerGamepadRumble(TriggerGamepadRumbleData data)
        {
            var currGamepad = Gamepad.current;
            if (currGamepad == null) return;
            
            currGamepad.ResetHaptics();
            currGamepad.SetMotorSpeeds(data.LeftMotorSpeed, data.RightMotorSpeed);
            currGamepad.ResumeHaptics();
            StartCoroutine(Helper.ExecuteAfter(() => {
                currGamepad.PauseHaptics();
            }, data.Duration));
        }
        
        private void OnDeviceChange(InputDevice device, InputDeviceChange change)
        {
            if (change is not (InputDeviceChange.Added or InputDeviceChange.Enabled or InputDeviceChange.Reconnected))
                return;
            
            OnDeviceActivity(device.GetType());
        }
        private void OnAnyButtonPress(InputControl inputControl)
        {
            OnDeviceActivity(inputControl.device.GetType());
        }

        private void OnDeviceActivity(Type deviceType)
        {
            var currentDeviceType = SetCurrentInputDeviceData.FromType(deviceType);

            if (currentDeviceType == m_previousDeviceClass)
                return;

            m_currentInputDeviceEvent.Invoke(new SetCurrentInputDeviceData()
            {
                Type = currentDeviceType,
            });

            m_previousDeviceClass = currentDeviceType;
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
