using UnityEngine;

namespace Fishing.SO
{
    [CreateAssetMenu(fileName = "FishingConfig", menuName = "br/Fishing/FishingConfig", order = 1)]
    public class FishingConfig : ScriptableObject
    {
        [field: SerializeField]
        public float StartCompletionRatio { get; set; } = 0.35f;
        
        [field: SerializeField]
        public Vector2 StartFishPos { get; set; } = new Vector2(0, 0.8f);
        
        [field: SerializeField]
        public float FishUiStartDecayThreshold { get; set; } = 0.1f;

        [field: SerializeField]
        public float FishUiIndicatorSpeed { get; set; } = 1.0f;

        [field: SerializeField]
        public Vector2 BarSize { get; set; } = new Vector2(50, 250);
    }
}
