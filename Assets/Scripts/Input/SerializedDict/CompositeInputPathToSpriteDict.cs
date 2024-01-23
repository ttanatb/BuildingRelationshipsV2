using System.Collections.Generic;
using System.Linq;
using Input.Structs;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Input.SerializedDict
{
    [System.Serializable]
    public class CompositeInputPathToSpriteDict : SerializableDictionary<InputCompositePath, Sprite>
    {
        public bool TryGetValue(IEnumerable<InputBinding> bindings, out Sprite mouseKeyboardIcon, out Sprite gamepadIcon)
        {
            gamepadIcon = null;
            mouseKeyboardIcon = null;
            foreach (var composite in Keys)
            {
                if (!composite.Paths.All(path => bindings.Any(b => b.path == path)))
                    continue;

                if (composite.Paths.Any(path => path.Contains("Keyboard") || path.Contains("Pointer")))
                    mouseKeyboardIcon = this[composite];
                
                if (composite.Paths.Any(path => path.Contains("Gamepad")))
                    gamepadIcon = this[composite];
            }
            return (gamepadIcon || mouseKeyboardIcon);
        }    
    }
}
