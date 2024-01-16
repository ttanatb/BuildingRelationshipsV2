using UnityEngine;
using Utilr.SoGameEvents;
using VolleyGame.Structs;

namespace VolleyGame.Events
{
    [CreateAssetMenu(fileName = "MoveVolleyCharacterEvent", menuName = "brVolley/MoveVolleyCharacterEvent", order = 1)]
    public class MoveVolleyCharacterEvent : SoCustomEvent<VolleyCharacterMovementData>
    {
        
    }
}
