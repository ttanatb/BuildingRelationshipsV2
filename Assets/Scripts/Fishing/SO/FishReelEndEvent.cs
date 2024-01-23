using UnityEngine;
using Utilr.SoGameEvents;
using Fishing.Structs;

namespace Fishing.SO
{
    [CreateAssetMenu(fileName = "FishReelEndEvent", menuName = "br/Fishing/FishReelEndEvent", order = 1)]
    public class FishReelEndEvent : SoCustomEvent<FishReelEndData>
    {
        
    }
}
