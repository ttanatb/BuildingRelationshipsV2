using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

namespace Input.Structs
{
    [System.Serializable]
    public struct SpritePerInputDevice
    {
        [field: SerializeField]
        public Sprite MouseKeyboard { get; set; }
        
        [field: SerializeField]
        public Sprite ControllerGeneric { get; set; }
        
        [field: SerializeField]
        public Sprite ControllerPs4 { get; set; }
        
        [field: SerializeField]
        public Sprite ControllerPs5 { get; set; }
        
        [field: SerializeField]
        public Sprite ControllerXbox { get; set; }
        
        [field: SerializeField]
        public Sprite ControllerNintendo { get; set; }

        public SpritePerInputDevice MergeWith(SpritePerInputDevice other)
        {
            var propInfos = typeof(SpritePerInputDevice).GetProperties();
            var result = new SpritePerInputDevice();
            object boxed = result;
            foreach (var prop in propInfos)
            {
                var thisValue = (Sprite)prop.GetValue(this);
                var otherValue = (Sprite)prop.GetValue(other);
                if (thisValue != null && otherValue != null)
                {
                    Debug.LogError($"Conflicting merge {prop} between {thisValue} & {otherValue}");
                }
                
                prop.SetValue(boxed, thisValue != null ? thisValue : otherValue);
            }

            return (SpritePerInputDevice)boxed;
        }

        public SpritePerInputDevice AssignIconBasedOnBinding(string bindingPath, Sprite icon)
        {
            if (bindingPath.Contains("Gamepad"))
            {
                ControllerGeneric = icon;
                ControllerPs4 = icon;
                ControllerPs5 = icon;
                ControllerXbox = icon;
                ControllerNintendo = icon;
            }
            else if (bindingPath.Contains("Keyboard") || bindingPath.Contains("Mouse"))
                MouseKeyboard = icon;
            else
            {
                Debug.LogError($"Cannot assign {bindingPath} to appropriate property");
            }
            return this;
        }
    }
}
