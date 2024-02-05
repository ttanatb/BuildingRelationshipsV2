using UI.Structs;
using UnityEngine;
using Utilr.SoGameEvents;

namespace UI.Events
{
    [CreateAssetMenu(fileName = "ShowInteractableMarkerEvent", menuName = "br/UI/ShowInteractableMarkerEvent", order = 1)]
    public class ShowInteractableMarkerEvent : SoCustomEvent<ShowInteractableMarkerData>
    {
        
    }
}
