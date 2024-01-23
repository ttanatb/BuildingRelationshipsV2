using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.Switch;
using UnityEngine.InputSystem.XInput;

namespace Input.Structs
{
    [System.Serializable]
    public struct SetCurrentInputDeviceData
    {
        public enum DeviceType
        {
            Invalid,
            MouseKeyboard,
            ControllerGeneric,
            ControllerPlaystationPs4,
            ControllerPlaystationPs5,
            ControllerXbox,
            ControllerNintendo,
        }

        public static DeviceType FromType(Type type)
        {
            if (type.IsSubclassOf(typeof(Mouse)) || type.IsSubclassOf(typeof(Keyboard)))
                return DeviceType.MouseKeyboard;
            if (type.IsSubclassOf(typeof(DualShockGamepad)))
                return DeviceType.ControllerPlaystationPs4;
            if (type.IsSubclassOf(typeof(DualSenseGamepadHID)))
                return DeviceType.ControllerPlaystationPs5;
            if (type.IsSubclassOf(typeof(XInputController)))
                return DeviceType.ControllerXbox;
            if (type.IsSubclassOf(typeof(SwitchProControllerHID)))
                return DeviceType.ControllerNintendo;
            if (type.IsSubclassOf(typeof(Gamepad)))
                return DeviceType.ControllerGeneric;

            throw new ArgumentOutOfRangeException($"Unsupported for type {type}");
        }
        
        [field: SerializeField]
        public DeviceType Type { get; set; }
    }
}
