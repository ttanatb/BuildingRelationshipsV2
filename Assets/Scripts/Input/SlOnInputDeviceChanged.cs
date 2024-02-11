using System;
using Input.SO;
using Input.Structs;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Input
{
    public class SlOnInputDeviceChanged : MonoBehaviour
    {
        [SerializeField]
        private InputIconDb m_inputIconDb = null;
        
        [SerializeField]
        private SetCurrentInputDeviceEvent m_inputDeviceEvent = null;
        
        [SerializeField]
        private InputActionReference m_input = null;
        

        [SerializeField]
        private SpritePerInputDevice m_sprites = new SpritePerInputDevice();

        private Image m_image = null;
        private SpriteRenderer m_spriteRenderer = null;
        
        // Optional component (to resize UI)
        private AdjustIconSize m_adjustIconSize = null;
        
        private void Start()
        {
            m_inputDeviceEvent.Event.AddListener(OnInputDeviceChanged);
            TryGetComponent(out m_image);
            TryGetComponent(out m_spriteRenderer);
            TryGetComponent(out m_adjustIconSize);
            
            Assert.IsTrue(m_image || m_spriteRenderer, 
                $"{gameObject}: requires either Image or SpriteRenderer component");
        }

        [Button]
        private void AssignSpriteFromInput()
        {
            var bindings = m_input.action.bindings;
            var result = m_sprites;
            foreach (var binding in bindings)
            {
                string path = binding.path;
                Debug.Log($"Looking through {path}");
                if (m_inputIconDb.Database.TryGetValue(path, out var icon))
                {
                    result = result.AssignIconBasedOnBinding(path, icon);
                }
                
                if (m_inputIconDb.ControllerDb.TryGetValue(path, out var controllerIcons))
                {
                    result = result.MergeWith(controllerIcons);
                }
            }
            
            if (m_inputIconDb.CompositeDb.TryGetValue(bindings, out var mkCompositeIcon, out var gamepadIcon))
            {
                if (mkCompositeIcon)
                {
                    result.MouseKeyboard = mkCompositeIcon;
                }

                if (gamepadIcon)
                {
                    result.ControllerGeneric = gamepadIcon;
                    result.ControllerPs4 = gamepadIcon;
                    result.ControllerPs5 = gamepadIcon;
                    result.ControllerXbox = gamepadIcon;
                    result.ControllerNintendo = gamepadIcon;
                }
            }

            m_sprites = result;
        }

        [Button]
        private void Clear()
        {
            m_sprites = new SpritePerInputDevice();
        }
        
        private void OnDestroy()
        {
            m_inputDeviceEvent.Event.RemoveListener(OnInputDeviceChanged);
        }

        private void OnInputDeviceChanged(SetCurrentInputDeviceData data)
        {
            var sprite = GetSpriteFromType(data.Type, m_sprites);
            if (m_image)
            {
                m_image.sprite = sprite;
                if (m_adjustIconSize)
                    m_adjustIconSize.ResizeFor(sprite);
            }

            if (m_spriteRenderer)
            {
                m_spriteRenderer.sprite = sprite;
            }
        }

        private static Sprite GetSpriteFromType(SetCurrentInputDeviceData.DeviceType type, SpritePerInputDevice sprites)
        {
            return type switch
            {
                SetCurrentInputDeviceData.DeviceType.MouseKeyboard => sprites.MouseKeyboard,
                SetCurrentInputDeviceData.DeviceType.ControllerGeneric => sprites.ControllerGeneric,
                SetCurrentInputDeviceData.DeviceType.ControllerPlaystationPs4 => sprites.ControllerPs4,
                SetCurrentInputDeviceData.DeviceType.ControllerPlaystationPs5 => sprites.ControllerPs5,
                SetCurrentInputDeviceData.DeviceType.ControllerXbox => sprites.ControllerXbox,
                SetCurrentInputDeviceData.DeviceType.ControllerNintendo => sprites.ControllerNintendo,
                SetCurrentInputDeviceData.DeviceType.Invalid => throw new ArgumentOutOfRangeException(),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}
