using UnityEngine;
using Utilr.SoGameEvents;
using VolleyGame.Structs;

namespace VolleyGame.Events
{
    [CreateAssetMenu(fileName = "DisplayVolleyTextEvent", menuName = "brVolley/DisplayVolleyTextEvent", order = 1)]
    public class DisplayVolleyTextEvent : SoCustomEvent<VolleyTextData>
    {
        
    }
}
