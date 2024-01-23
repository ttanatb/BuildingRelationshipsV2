using UnityEngine;

namespace Fishing.Structs
{
    [System.Serializable]
    public struct FishReelStartData
    {
        [field: SerializeField]
        public FishData FishData { get; set; }
		
        [field: SerializeField]
        public Fish Fish { get; set; }
    }
}
