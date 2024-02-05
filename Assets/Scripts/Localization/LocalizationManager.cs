using System;
using System.Linq;
using Dialogue.SerializedDict;
using NaughtyAttributes;
using UnityEngine;
using Util.Localization.Enum;
using Util.Localization.SO;
using Util.Localization.Structs;
using Utilr.Attributes;

namespace Util.Localization
{
    public class LocalizationManager : Utilr.Singleton<LocalizationManager>
    {
        [SerializeField] [Expandable] [IncludeAllAssetsWithType] 
        private LocalizedStringTable[] m_tables = null;

        private StringToStringDict m_currentTable = null;
        private LanguageCode m_languageCode = LanguageCode.English;

        private void Awake()
        {
            SetLanguage(LanguageCode.English);
        }

        public void SetLanguage(LanguageCode code)
        {
            m_languageCode = code;
            m_currentTable = m_tables.First(table => table.LanguageCode == code).Data;
        }

        public string GetLine(LocalizedText key)
        {
            return m_currentTable[key.Key];
        }
    }
}
