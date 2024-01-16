using UI.Structs;
using UnityEngine;
using Utilr.SoGameEvents;

namespace UI.Events
{
    [CreateAssetMenu(fileName = "PositionUiEvent", menuName = "br/PositionUiEvent", order = 1)]
    public class PositionUiEvent : SoCustomEvent<PositionUiData>
    {
        
    }
}
