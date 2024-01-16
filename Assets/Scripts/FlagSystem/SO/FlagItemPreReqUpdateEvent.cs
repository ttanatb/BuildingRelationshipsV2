using FlagSystem.Structs;
using UnityEngine;
using Utilr.SoGameEvents;

namespace FlagSystem.SO
{
    [CreateAssetMenu(fileName = "FlagItemPreReqUpdateEvent", menuName = "br/FlagItemPreReqUpdateEvent", order = 1)]
    public class FlagItemPreReqUpdateEvent : SoCustomEvent<FlagItemPreReqUpdateStatus>
    {
        
    }
}
