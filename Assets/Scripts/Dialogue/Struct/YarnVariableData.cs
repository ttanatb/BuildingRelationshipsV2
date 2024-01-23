using Dialogue.SerializedDict;
using UnityEngine;

namespace Dialogue.Struct
{
    [System.Serializable]
    public struct YarnVariableData
    {
        [field: SerializeField]
        public StringToFloatDict FloatDict { get; set; }

        [field: SerializeField] 
        public StringToStringDict StringDict { get; set; } 
        
        [field: SerializeField]
        public StringToBoolDict BoolDict { get; set; }
    }
}
