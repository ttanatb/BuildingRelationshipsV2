using System;
using UnityEngine;

namespace Movement.Structs
{
    [System.Serializable]
    public struct DebugContactData
    {
        [field: SerializeField]
        public Vector3 Position { get; set; }
        
        [field: SerializeField]
        public Vector3 Normal { get; set; }
        
        [field: SerializeField]
        public Vector3 Impulse { get; set; }
        
        [field: SerializeField]
        public Vector3 RelativeVelocity { get; set; }
        
        [field: SerializeField]
        public TerrainCollisionData? TerrainData { get; set; }
    }
}
