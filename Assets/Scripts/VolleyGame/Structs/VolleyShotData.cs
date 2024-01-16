using UnityEngine;

namespace VolleyGame.Structs
{
    [System.Serializable]
    public struct VolleyShotData
    {
        [field: SerializeField]
        public float Duration { get; set; }
        
        [field: SerializeField]
        public Vector3 StartPos { get; set; }
        
        [field: SerializeField]
        public Vector3 EndPos { get; set; }
        
        [field: SerializeField]
        public float PeakHeight { get; set; }
        
        [field: SerializeField]
        public AnimationCurve HorizontalCurve { get; set; }

        [field: SerializeField]
        public AnimationCurve VerticalCurve { get; set; }
    }
}
