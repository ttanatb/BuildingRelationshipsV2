using UnityEngine;

namespace Movement.Structs
{
    [System.Serializable]
    public struct TerrainCollisionData
    {
        [field: SerializeField]
        public Vector3 Normal { get; set; }
        
        [field: SerializeField]
        public float Height { get; set; }
    }
}
