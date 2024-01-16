using FlagSystem.Structs;
using UnityEngine;
using Utilr.SoGameEvents;

namespace FlagSystem.SO
{
    [CreateAssetMenu(fileName = "FlagGraphUpdateEvent", menuName = "br/FlagGraphUpdateEvent", order = 1)]
    public class FlagGraphUpdateEvent : SoCustomEvent<FlagGraphUpdateStatus>
    {
        
    }
}
