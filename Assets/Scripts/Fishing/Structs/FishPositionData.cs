using UnityEngine;

namespace Fishing.Structs
{
    [System.Serializable]
    public struct FishPositionData
    {
        [field: SerializeField]
        public Vector2 Position { get; set; }
    }
}
