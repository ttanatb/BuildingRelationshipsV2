using UnityEngine;
using Utilr.SoGameEvents;

namespace GameEvents.Fishing
{
    [CreateAssetMenu(fileName = "UpdateFishProgressEvent", menuName = "br/Fishing/UpdateFishProgressEvent", order = 1)]
    public class UpdateFishProgressEvent : SoCustomEvent<float>
    {

    }
}
