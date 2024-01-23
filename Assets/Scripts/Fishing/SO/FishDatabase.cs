using Fishing.SerializedDict;
using UnityEngine;

namespace Fishing.SO
{
    [CreateAssetMenu(fileName = "FishDatabase", menuName = "br/Fishing/FishDatabase", order = 1)]
    public class FishDatabase : ScriptableObject
    {
        [field: SerializeField]
        public ItemIdToFishDataDict Dict { get; set; }    
    }
}
