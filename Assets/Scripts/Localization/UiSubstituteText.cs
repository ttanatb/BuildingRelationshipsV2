using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Util.Localization.Structs;

namespace Util.Localization
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class UiSubstituteText : MonoBehaviour
    {
        private void Start()
        {
            TryGetComponent(out TextMeshProUGUI text);
            text.text = new LocalizedText()
            {
                Key = text.text,
            }.GetText();
            LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
        }
    }
}
