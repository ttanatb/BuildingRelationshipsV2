using UnityEngine;
using Utilr.SoGameEvents;

namespace UI.Structs
{
    [CreateAssetMenu(fileName = "SoUpdateFishUiEvent", menuName = "soEventsBr/SoUpdateFishUiEvent", order = 1)]
    public class SoUpdateItemEvent : SoCustomEvent<UpdateItemStatus>
    {
        
    }
}
