using UnityEngine;
using Util.Localization.Structs;

namespace UI.Structs
{
    [System.Serializable]
    public struct ShowInteractableMarkerData
    {
        [field: SerializeField]
        public LocalizedText Text { get; set; } 
    }
}
