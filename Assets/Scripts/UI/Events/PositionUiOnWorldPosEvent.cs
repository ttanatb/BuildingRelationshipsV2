using UI.Structs;
using UnityEngine;
using Utilr.SoGameEvents;

namespace UI.Events
{
    [CreateAssetMenu(fileName = "PositionUiOnWorldPos", menuName = "br/UI/PositionUiOnWorldPos", order = 1)]
    public class PositionUiOnWorldPosEvent : SoCustomEvent<PositionUiOnWorldPosData>
    {
        
    }
}
