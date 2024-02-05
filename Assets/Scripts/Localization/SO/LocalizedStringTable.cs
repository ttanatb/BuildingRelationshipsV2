using System.Collections.Generic;
using Dialogue.SerializedDict;
using NaughtyAttributes;
using UnityEngine;
using Util.Localization.Enum;

namespace Util.Localization.SO
{
    [CreateAssetMenu(fileName = "LocalizedStringTable", menuName = "so/Localization/LocalizedStringTable", order = 1)]
    public class LocalizedStringTable : ScriptableObject
    {
        [field: SerializeField]
        public LanguageCode LanguageCode { get; set; }
        
        [field: SerializeField]
        public StringToStringDict Data { get; set; }

        [Button]
        private void Sort()
        {
            var sortedDictionary = new SortedDictionary<string, string>(Data);
            Data.CopyFrom(sortedDictionary);
        }
    }
}
