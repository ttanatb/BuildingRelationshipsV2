using UnityEngine;

namespace UI.Structs
{
    [System.Serializable]
    public struct PositionUiOnWorldPosData
    {  
        [field: SerializeField]
        public Vector3 Position { get; set; }
    }
}
