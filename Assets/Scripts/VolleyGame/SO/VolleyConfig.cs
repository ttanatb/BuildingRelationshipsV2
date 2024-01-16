using UnityEngine;

namespace VolleyGame.SO
{
    [CreateAssetMenu(fileName = "VolleyConfig", menuName = "brVolley/VolleyConfig", order = 1)]
    public class VolleyConfig : ScriptableObject
    {
        [field: SerializeField]
        public float FpsCamSensitivity { get; set; } = 1.0f;

        [field: SerializeField]
        public float FpsCamInputFactor { get; set; } = 1.0f;
        
        [field: SerializeField]
        public float VolleyShotFastDuration { get; set; } = 3.5f;

        [field: SerializeField]
        public float VolleyShotMediumDuration { get; set; } = 5.0f;

        [field: SerializeField]
        public float VolleyShotSlowDuration { get; set; } = 7.5f;
        
        [field: SerializeField]
        public float VolleyShotHeightLow { get; set; } = 5.5f;
        
        [field: SerializeField]
        public float VolleyShotHeightHigh { get; set; } = 8.5f;
        
        [field: SerializeField]
        public AnimationCurve VolleyShotHorizontalCurve { get; set; }
                
        [field: SerializeField]
        public AnimationCurve VolleyShotVerticalCurveSlow { get; set; }
        
        [field: SerializeField]
        public AnimationCurve VolleyShotVerticalCurveSmash { get; set; }

        [field: SerializeField]
        public AnimationCurve VolleyOpponentMoveCurve { get; set; }
    }
}
