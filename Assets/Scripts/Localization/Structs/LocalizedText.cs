using UnityEngine;

namespace Util.Localization.Structs
{
    [System.Serializable]
    public struct LocalizedText
    {
        [field: SerializeField]
        public string Key { get; set; }

        private static LocalizationManager m_localizationManager = null;

        public string GetText()
        {
            if (m_localizationManager == null)
                m_localizationManager = LocalizationManager.Instance;

            return m_localizationManager.GetLine(this);
        }
    }
}
