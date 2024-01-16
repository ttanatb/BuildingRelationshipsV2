using UnityEngine;
using Utilr.SoGameEvents;
using VolleyGame.Structs;

namespace VolleyGame.Events
{
    [CreateAssetMenu(fileName = "FireVolleyShotEvent", menuName = "brVolley/FireVolleyShotEvent", order = 1)]
    public class FireVolleyShotEvent : SoCustomEvent<VolleyShotData>
    {
        
    }
}
