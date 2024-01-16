using UnityEngine;

namespace UI.Structs
{
    [CreateAssetMenu(fileName = "UiFishConfig", menuName = "soEventsBr/UiFishConfig", order = 1)]
    [System.Serializable]
    public class UiFishConfig : ScriptableObject
    {
        [field: SerializeField]
        public Color HiddenMediaBoxColor { get; set; } = Color.black;

        [field: SerializeField]
        public Color HiddenMainBoxColor { get; set; } = Color.black;

        [field: SerializeField]
        public Color HiddenBgBoxColor { get; set; } = Color.black;
        
        [field: SerializeField]
        public Color HiddenFishSpriteColor { get; set; } = Color.black;
        
        [field: SerializeField]
        public Color AvailableColor { get; set; } = Color.white;
        
        [field: SerializeField]
        public string HiddenText { get; set; } = "???";
    }
}
