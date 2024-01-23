using UnityEngine;

namespace Input.Structs
{
    [System.Serializable]
    public struct TriggerGamepadRumbleData
    {
        [field: SerializeField]
        public float LeftMotorSpeed { get; set; }
        
        [field: SerializeField]
        public float RightMotorSpeed { get; set; }
        
        [field: SerializeField]
        public float Duration { get; set; }
    }
}
