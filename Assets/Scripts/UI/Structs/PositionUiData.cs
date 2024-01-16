using UnityEngine;

namespace UI.Structs
{
    [System.Serializable]
    public struct PositionUiData
    {
        [field: SerializeField]
        public TextAnchor Anchor { get; set; }
        
        [field: SerializeField]
        public Vector2 Offset { get; set; }
    }
}
