using Settings.Structs;
using UnityEngine;
using Utilr.SoGameEvents;

namespace Settings.SO
{
    [CreateAssetMenu(fileName = "SettingsUpdateEvent", menuName = "so/settings/SettingsUpdateEvent", order = 1)]
    public class SettingsUpdateEvent : SoCustomEvent<SettingsData>
    {
        
    }
}
