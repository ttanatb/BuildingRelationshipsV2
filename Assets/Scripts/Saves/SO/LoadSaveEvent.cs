using Saves.Structs;
using UnityEngine;
using Utilr.SoGameEvents;

namespace Saves.SO
{
    [CreateAssetMenu(fileName = "OnSaveLoaded", menuName = "so/save/OnSaveLoaded", order = 1)]
    public class LoadSaveEvent : SoCustomEvent<SaveData>
    {
        
    }
}
