using UnityEngine;
using Utilr.SoGameEvents;

namespace Dialogue.SO
{
    [CreateAssetMenu(fileName = "PositionPlayerEvent", menuName = "br/PositionPlayerEvent", order = 1)]
    public class PositionPlayerEvent : SoCustomEvent<Transform>
    {
        
    }
}
