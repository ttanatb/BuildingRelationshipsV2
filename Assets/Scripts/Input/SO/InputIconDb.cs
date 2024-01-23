using Input.SerializedDict;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Input.SO
{
    [CreateAssetMenu(fileName = "InputIconDb", menuName = "br/Input/InputIconDb", order = 1)]
    public class InputIconDb : ScriptableObject
    {
        [field: SerializeField] 
        public InputPathToSpriteDict Database { get; set; }
        
        [field: SerializeField] 
        public InputPathToSpriteGroupDict ControllerDb { get; set; }
        
        [field: SerializeField]
        public CompositeInputPathToSpriteDict CompositeDb { get; set; }
        
    }
}
