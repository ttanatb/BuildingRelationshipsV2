using Fishing.Structs;
using UnityEngine;
using Utilr.SoGameEvents;

namespace GameEvents.Fishing
{
    [CreateAssetMenu(fileName = "UpdateFishPositionEvent", menuName = "br/Fishing/UpdateFishPositionEvent", order = 1)]
    public class UpdateFishPositionEvent : SoCustomEvent<FishPositionData>
    {
        
    }
}
