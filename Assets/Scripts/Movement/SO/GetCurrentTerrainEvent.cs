using UnityEngine;
using Utilr.SoGameEvents;

namespace Movement.SO
{
    [CreateAssetMenu(fileName = "GetCurrentTerrainEvent", menuName = "br/GetCurrentTerrainEvent", order = 1)]
    public class GetCurrentTerrainEvent : SoCustomEvent<Terrain>
    {
        
    }
}
