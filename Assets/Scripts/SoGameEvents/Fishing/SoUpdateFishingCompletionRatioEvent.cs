using UnityEngine;
using Utilr.SoGameEvents;

namespace GameEvents.Fishing
{
    [CreateAssetMenu(fileName = "SoUpdateFishingCompletionRatioEvent", menuName = "soEventsBr/SoUpdateFishingCompletionRatioEvent", order = 1)]
    public class SoUpdateFishingCompletionRatioEvent : SoCustomEvent<float>
    {

    }
}
