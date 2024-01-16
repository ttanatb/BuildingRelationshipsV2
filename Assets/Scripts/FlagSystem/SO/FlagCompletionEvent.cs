using UnityEngine;
using Utilr.SoGameEvents;

namespace FlagSystem.SO
{
    [CreateAssetMenu(fileName = "FlagCompletionEvent", menuName = "br/FlagCompletionEvent", order = 1)]
    public class FlagCompletionEvent : SoCustomEvent<FlagNode>
    {
        
    }
}
