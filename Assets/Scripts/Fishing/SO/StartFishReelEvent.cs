using Fishing.Structs;
using UnityEngine;
using Utilr.SoGameEvents;

namespace Fishing.SO
{
    [CreateAssetMenu(fileName = "FishReelStartEvent", menuName = "br/Fishing/FishReelStartEvent", order = 1)]
    public class StartFishReelEvent : SoCustomEvent<FishReelStartData>
    {
        
    }
}
